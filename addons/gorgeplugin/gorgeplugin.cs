#if TOOLS
using Godot;
using System;

[Tool]
public partial class gorgeplugin : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		var script = GD.Load<Script>("res://addons/gorgeplugin/GamePlayer.cs");
		AddCustomType("GamePlayer","Node",script,new Texture2D());
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("GamePlayer");
	}
}
#endif
