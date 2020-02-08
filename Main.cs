using Godot;
using System;

public class Main : Spatial
{
	public static readonly Color BackgroundColor = Color.Color8(0, 88, 88, 255);
	public const float ShotRange = 64;
	public ARVRInterface ARVRInterface { get; set; }
	public ARVROrigin ARVROrigin { get; set; }
	public ARVRCamera ARVRCamera { get; set; }
	public ARVRController LeftController { get; set; }
	public ARVRController RightController { get; set; }
	public VirtualScreen VirtualScreen { get; set; }
	public Line3D Line3D { get; set; }
	public MeshInstance Cube = new MeshInstance()
	{
		Mesh = new CubeMesh()
		{
			Size = new Vector3(VirtualScreen.PixelWidth, VirtualScreen.PixelWidth, VirtualScreen.PixelWidth),
		},
		MaterialOverride = new SpatialMaterial()
		{
			AlbedoColor = Color.Color8(255, 0, 255, 255), // Purple
			FlagsUnshaded = true,
			FlagsDoNotReceiveShadows = true,
			FlagsDisableAmbientLight = true,
			FlagsTransparent = false,
			ParamsCullMode = SpatialMaterial.CullMode.Disabled,
			ParamsSpecularMode = SpatialMaterial.SpecularMode.Disabled,
		},
	};

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
			Transform = new Transform(Basis.Identity, new Vector3(0f, VirtualScreen.Height / 2f, -1f)),
		});

		AddChild(Line3D = new Line3D()
		{
			Color = Color.Color8(255, 0, 0, 255),
		});

		AddChild(Cube);
	}

	public static Vector3 ARVRControllerDirection(Basis basis) => -basis.z.Rotated(basis.x.Normalized(), -Mathf.Pi * 3f / 16f).Normalized();
	public Vector3 LeftControllerDirection => ARVRControllerDirection(LeftController.GlobalTransform.basis);
	public Vector3 RightControllerDirection => ARVRControllerDirection(RightController.GlobalTransform.basis);

	public override void _PhysicsProcess(float delta)
	{
		if (RightController.IsButtonPressed((int)Godot.JoystickList.VrTrigger) > 0)
		{
			if (!Shooting)
			{
				Line3D.Vertices = new Vector3[] {
						RightController.GlobalTransform.origin,
						RightController.GlobalTransform.origin + RightControllerDirection * ShotRange
					};
				Godot.Collections.Dictionary result = GetWorld().DirectSpaceState.IntersectRay(
					Line3D.Vertices[0],
					Line3D.Vertices[1]
					);

				GD.Print("Shooting! Time: " + DateTime.Now);
				if (result.Count > 0)
				{
					CollisionObject collider = (CollisionObject)result["collider"];
					GD.Print(
						((CollisionShape)collider.ShapeOwnerGetOwner(collider.ShapeFindOwner((int)result["shape"]))).Name
						);
					GD.Print(result);
					Vector3 position = (Vector3)result["position"];
					Cube.Transform = new Transform(Basis.Identity, position);
					Vector3 localPosition = VirtualScreen.ToLocal(position);
					VirtualScreen.TargetPosition = new Vector2(localPosition.x, localPosition.y);
				}
				else
					GD.Print("Hit nothing! :(");
				Shooting = true;
			}
		}
		else
			Shooting = false;
	}

	private bool Shooting = false;
}
