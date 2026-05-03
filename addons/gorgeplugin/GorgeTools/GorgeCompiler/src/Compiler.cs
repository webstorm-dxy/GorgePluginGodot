using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Gorge.GorgeCompiler.CompileContext;
using Gorge.GorgeCompiler.CompileContext.Block;
using Gorge.GorgeCompiler.CompileContext.Scope;
using Gorge.GorgeCompiler.Expression;
using Gorge.GorgeCompiler.Visitors;
using Gorge.Native.Gorge;
using GorgeCompiler.AntlrGen;

namespace Gorge.GorgeCompiler
{
    public static class Compiler
    {
        /// <summary>
        /// 执行编译
        /// </summary>
        /// <param name="sourceFiles">源文件列表</param>
        /// <returns>编译完成的编译上下文</returns>
        public static ClassImplementationContext Compile(IEnumerable<SourceCodeFile> sourceFiles)
        {
            // 语法树解析
            var sourceFileParseTrees = new List<IParseTree>(); // 源文件语法树表
            foreach (var sourceFile in sourceFiles)
            {
                var inputStream = new AntlrInputStream(sourceFile.Code)
                {
                    name = sourceFile.Path
                };
                var lexer = new GorgeLexer(inputStream, new LogWriter("Info"),
                    new LogWriter("Error"));
                var tokens = new CommonTokenStream(lexer);
                var parser = new GorgeParser(tokens, new LogWriter("Info"), new LogWriter("Error"));
                var tree = parser.sourceFile();
                sourceFileParseTrees.Add(tree);
            }

            // 编译上下文
            var compileContext = new ClassImplementationContext();

            // 一轮编译
            var typeIdentifierVisitor = new TypeIdentifierVisitor(compileContext.GlobalScope);
            foreach (var sourceFile in sourceFileParseTrees)
            {
                typeIdentifierVisitor.Visit(sourceFile);
            }

            // 二轮编译
            var typeExtensionVisitor = new TypeExtensionVisitor();
            typeExtensionVisitor.CompileNamespace(compileContext.GlobalScope);

            // 三轮编译
            var typeDeclarationVisitor = new TypeDeclarationVisitor();
            var implementationCompileTasks = typeDeclarationVisitor.CompileNamespace(compileContext.GlobalScope);

            // 四轮编译
            foreach (var implementationCompileTask in implementationCompileTasks)
            {
                implementationCompileTask.DoCompile(compileContext, false, true);
            }

            compileContext.FreezeImplementation();

            return compileContext;
        }

        public static Injector DynamicCompileObjectInjector(this ClassImplementationContext context,
            string injectorCode)
        {
            var inputStream = new AntlrInputStream(injectorCode)
            {
                name = "Dynamic"
            };
            var lexer = new GorgeLexer(inputStream, new LogWriter("Info"),
                new LogWriter("Error"));
            var tokens = new CommonTokenStream(lexer);
            var parser = new GorgeParser(tokens, new LogWriter("Info"), new LogWriter("Error"));

            var dynamicClassBase = context.GlobalScope.SubNamespaces.Values.First(v => v.NamespaceName == "Gorge")
                .Classes.Values.First(c => c.ClassSymbol.Identifier == "Injector");

            var block = new CodeBlockScope(BlockContextType.StaticMethod, dynamicClassBase.ClassSymbol, null,
                dynamicClassBase);

            var result = (Injector) new ExpressionVisitor(block, false).Visit(parser.expression())
                .Assert<ObjectImmediate>()
                .CompileConstantValue;

            return result;
        }
    }
}