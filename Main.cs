using Godot;
using System;

public class Main : Spatial
{
	public ARVRInterface ARVRInterface { get; set; }
	public ARVROrigin ARVROrigin { get; set; }
	public ARVRCamera ARVRCamera { get; set; }
	public ARVRController LeftController { get; set; }
	public ARVRController RightController { get; set; }
	public VirtualScreen VirtualScreen { get; set; }
	public Line3D Line3D { get; set; }
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
		LeftController.AddChild((Spatial)GD.Load<PackedScene>("res://OQ_Toolkit/OQ_ARVRController/models3d/OculusQuestTouchController_Left.gltf").Instance());
		RightController.AddChild((Spatial)GD.Load<PackedScene>("res://OQ_Toolkit/OQ_ARVRController/models3d/OculusQuestTouchController_Right.gltf").Instance());

		ARVRInterface = ARVRServer.FindInterface(OS.GetName().Equals("Android") ? "OVRMobile" : "OpenVR");

		if (ARVRInterface != null && ARVRInterface.Initialize())
			GetViewport().Arvr = true;

		AddChild(VirtualScreen = new VirtualScreen()
		{
			Transform = new Transform(Basis.Identity, new Vector3(0, VirtualScreen.Height / 2f, -VirtualScreen.Height / 2f)),
		});

		AddChild(Line3D = new Line3D()
		{
			Color = Color.Color8(255, 0, 0, 255),
		});
	}

	public override void _PhysicsProcess(float delta)
	{

	}
}
