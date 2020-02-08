using Godot;
using System;

public class Main : Spatial
{
	public ARVRInterface ARVRInterface { get; set; }
	public ARVROrigin ARVROrigin { get; set; }
	public ARVRCamera ARVRCamera { get; set; }
	public ARVRController LeftController { get; set; }
	public ARVRController RightController { get; set; }

	public static readonly Color BackgroundColor = Color.Color8(0, 88, 88, 255);

	public override void _Ready()
	{
		VisualServer.SetDefaultClearColor(BackgroundColor);
		AddChild(new WorldEnvironment()
		{
			Environment = new Godot.Environment()
			{
				BackgroundColor = BackgroundColor,
				BackgroundMode = Godot.Environment.BGMode.Color,
			},
		});
		AddChild(ARVROrigin = new ARVROrigin());
		ARVROrigin.AddChild(ARVRCamera = new ARVRCamera()
		{
			Current = true,
		});
		ARVROrigin.AddChild(LeftController = new ARVRController()
		{
			ControllerId = 1,
		});
		ARVROrigin.AddChild(RightController = new ARVRController()
		{
			ControllerId = 2,
		});

		ARVRInterface = ARVRServer.FindInterface(OS.GetName().Equals("Android") ? "OVRMobile" : "OpenVR");

		if (ARVRInterface != null && ARVRInterface.Initialize())
			GetViewport().Arvr = true;
	}

	public override void _PhysicsProcess(float delta)
	{

	}
}
