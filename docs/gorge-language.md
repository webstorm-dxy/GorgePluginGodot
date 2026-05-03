# Gorge Language: Compiler & Runtime

Gorge 是一门自定义的面向对象脚本语言，专为音频游戏谱面设计。源文件使用 `.g` 扩展名，通过 ANTLR4 编译为中间代码，由 `IntermediateCodeVirtualMachine` 执行。

## 语法定义 (ANTLR4)

Gorge 的语法分散在四个 `.g4` 文件中：

| 文件 | 内容 |
|------|------|
| `Gorge.g4` | 顶层语法：源文件、类型声明（类/接口/枚举/委托/命名空间） |
| `GorgeStatement.g4` | 语句语法：if/while/for/switch/return/break/continue |
| `GorgeExpression.g4` | 表达式语法：算术/逻辑/比较/赋值/调用/lambda |
| `GorgeLexerRules.g4` | 词法规则：关键字、操作符、字面量 |

## 编译器管线 (Compiler.cs)

编译分四轮进行：

```
源文件 (.g)
    │
    ▼
词法分析 (GorgeLexer) → Token流
    │
    ▼
语法分析 (GorgeParser) → AST (sourceFileParseTrees)
    │
    ├── 第一轮编译: TypeIdentifierVisitor
    │   收集所有类型定义（类/接口/枚举/委托），建立全局作用域符号表
    │
    ├── 第二轮编译: TypeExtensionVisitor
    │   解析继承链、接口实现、类型引用
    │
    ├── 第三轮编译: TypeDeclarationVisitor
    │   编译方法体，生成 ImplementationCompileTask 列表
    │
    └── 第四轮编译: ImplementationCompileTask.DoCompile()
        生成 IntermediateCode[] 并冻结编译上下文
    │
    ▼
ClassImplementationContext（编译产物）
```

### 编译上下文 (CompileContext)

整个编译过程围绕 `ClassImplementationContext` 展开，核心数据结构包括：

**作用域体系 (Scope)**：
- `GlobalScope` — 根作用域，包含所有命名空间
- `NamespaceScope` — 命名空间，包含类/接口/枚举
- `ClassScope` — 类作用域，包含字段、方法、构造方法、Injector 字段
- `MethodScope` / `MethodGroupScope` — 方法及其重载组
- `CodeBlockScope` — 代码块局部作用域
- `LambdaCodeBlockScope` — Lambda 闭合作用域
- `SymbolScope` — 符号作用域基类

**符号体系 (Symbol)**：
- `ClassSymbol` / `InterfaceSymbol` / `EnumSymbol` / `NamespaceSymbol`
- `MethodSymbol` / `ConstructorSymbol` — 方法和构造方法的编译时表示
- `FieldSymbol` / `InjectorFieldSymbol` — 字段和注入器字段
- `VariableSymbol` / `LocalSymbol` — 变量和局部变量
- `TypeSymbol` / `GenericsSymbol` — 类型和泛型
- `DelegateFieldSymbol` — 委托字段

**修饰符 (Modifier)**：
- `ModifierType` 枚举 + `Modifier` 位掩码
- 支持 public/private/protected/static/abstract/override/virtual

## 表达式系统

Gorge 的表达式 AST 遵循运算符优先级层级组织：

```
ConditionalExpression     ← (条件 ? 真值 : 假值)
    │
AssignmentExpression     ← 赋值（字段/数组/局部变量/注入器字段）
    │
LogicalExpression        ← && / ||
    │
EqualityExpression       ← == / !=
    │
ComparisonExpression     ← < / > / <= / >=
    │
AdditionExpression       ← + / - / Calculate (自定义运算?)
    │
UnaryLeftExpression      ← 数组访问 / 方法调用 / 注入器字段访问
    │
UnaryRightExpression     ← - / ! / 类型转换
    │
PrimaryExpression        ← this / lambda / 字面量 / 类型引用 / 局部引用
```

表达式编译最终生成 `ValueExpression`（含 `IntermediateCode[]`）。

## 中间代码 (IntermediateCode)

所有 Gorge 代码最终被编译为中间代码指令序列。每条指令包含 4 个字段：

```csharp
class IntermediateCode {
    Address Result;          // 结果存储地址
    IntermediateOperator Operator;  // 操作码
    IOperand Left;           // 左操作数（含义因操作码而异）
    IOperand Right;          // 右操作数（含义因操作码而异）
}
```

### 操作码分类 (70+ 条指令)

**域内运算** — 仅操作本地变量：
- `LocalIntAssign` / `LocalFloatAssign` / `LocalBoolAssign` / `LocalStringAssign` / `LocalObjectAssign`
- `IntOpposite` / `FloatOpposite`
- `IntAddition` / `FloatAddition` / `StringAddition`
- `IntSubtraction` / `FloatSubtraction`
- `IntMultiplication` / `FloatMultiplication`
- `IntDivision` / `FloatDivision`
- `IntRemainder` / `FloatRemainder`
- `IntLess` / `FloatLess` / `IntGreater` / `FloatGreater` / ...
- `IntEquality` / `FloatEquality` / `BoolEquality` / `StringEquality` / `ObjectEquality`
- `LogicalAnd` / `LogicalOr` / `LogicalNot`
- `IntCastToFloat` / `FloatCastToInt` / `IntCastToString` / `FloatCastToString` / `BoolCastToString` / `ObjectCastToObject`
- `LoadThis`

**对象运算** — 操作 GorgeObject 字段：
- `LoadIntField` / `LoadFloatField` / `LoadBoolField` / `LoadStringField` / `LoadObjectField`
- `SetIntField` / `SetFloatField` / `SetBoolField` / `SetStringField` / `SetObjectField`
- `LoadIntInjectorField` / `LoadFloatInjectorField` / `LoadBoolInjectorField` / `LoadStringInjectorField` / `LoadObjectInjectorField`
- `SetIntInjectorField` / `SetFloatInjectorField` / `SetBoolInjectorField` / `SetStringInjectorField` / `SetObjectInjectorField`

**调用**：
- `SetIntParameter` / `SetFloatParameter` / `SetBoolParameter` / `SetStringParameter` / `SetObjectParameter`
- `LoadIntParameter` / `LoadFloatParameter` / `LoadBoolParameter` / `LoadStringParameter` / `LoadObjectParameter`
- `InvokeMethod` / `InvokeStaticMethod` / `InvokeInterfaceMethod` / `InvokeDelegate`
- `InvokeConstructor` / `InvokeInjectorConstructor` / `DoConstruct`
- `GetReturnInt` / `GetReturnFloat` / `GetReturnBool` / `GetReturnString` / `GetReturnObject`
- `SetInjector` / `LoadInjector`
- `ConstructDelegate`
- 数组构造：`InvokeIntArrayConstructor` / `InvokeFloatArrayConstructor` / `InvokeBoolArrayConstructor` / `InvokeStringArrayConstructor` / `InvokeObjectArrayConstructor`

**控制流**：
- `Jump` / `JumpIfFalse` / `JumpIfTrue` — 支持回填跳转目标
- `Nop`

**退出**：
- `ReturnInt` / `ReturnFloat` / `ReturnBool` / `ReturnString` / `ReturnObject` / `ReturnVoid`

## 虚拟机 (IntermediateCodeVirtualMachine)

基于栈的虚拟机，包含 5 个类型化操作数栈：

```csharp
class IntermediateCodeVirtualMachine {
    VmStack<int> _intStack;
    VmStack<float> _floatStack;
    VmStack<bool> _boolStack;
    VmStack<string> _stringStack;
    VmStack<GorgeObject> _objectStack;
}
```

执行流程：
1. 在栈上分配局部变量空间（Push）
2. 按 programCounter 顺序执行中间代码
3. 遇到 Return 指令时退出
4. 弹出局部变量空间（Pop）

**操作数解析**：
- `Immediate`（立即数，值类型或编译时常量 GorgeObject）→ 直接取值
- `Address`（地址，指向栈上的变量槽）→ 从栈读取

**方法调用**：
- 通过 `InvokeParameterPool` 静态类在方法间传递参数和返回值
- Injector 同样通过 ParameterPool 传递
- 支持静态方法调用：通过 `ClassName → GorgeClass → InvokeStaticMethod(id)` 路径

## Gorge Language Runtime (GorgeLanguageRuntime)

`GorgeLanguageRuntime` 是 Gorge 程序运行时的核心：

```csharp
class GorgeLanguageRuntime : IImplementationBase {
    GorgeClass[] Classes;        // 所有类的实现
    GorgeEnum[] Enums;           // 所有枚举
    GorgeInterface[] Interfaces;  // 所有接口
    IntermediateCodeVirtualMachine Vm;  // 虚拟机实例
}
```

它合并 Native 侧（通过 `[GorgeNativeClass]` 等属性扫描的程序集本地实现）和编译产物（`.g` 源码编译的 ClassImplementationContext）的类定义，提供统一的类型反射和类型转换判断能力。

## 优化器 (IntermediateCodeOptimizer)

编译器在生成中间代码后，会经过 `IntermediateCodeOptimizer` 进行基本块优化：
- `BasicBlock` — 将中间代码划分为基本块
- `BasicBlockDag` — 构建 DAG 进行局部优化
- `Expression` — 表达式级别的优化

## 代码块 (CodeBlock)

Gorge 语言的控制流语句在编译时映射为 `ICodeBlock` 实现：
- `NormalBlock` — 顺序代码块
- `IfBlock` — if/else 分支
- `ForBlock` — for 循环
- `WhileBlock` — while 循环
- `DoWhileBlock` — do-while 循环
- `SwitchBlock` — switch 分支
