using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class VirtualScreen : StaticBody
{
    public const float Height = 2.7f;
    public CollisionShape CollisionShape { get; set; }
    public MeshInstance MeshInstance { get; set; }

    public VirtualScreen()
    {
        MeshInstance = new MeshInstance()
        {
            Mesh = new QuadMesh()
            {
                Size = new Vector2(Height, Height),
            },
            MaterialOverride = new SpatialMaterial()
            {
                AlbedoTexture = (Texture)GD.Load("res://icon.png"),
                FlagsUnshaded = true,
                FlagsDoNotReceiveShadows = true,
                FlagsDisableAmbientLight = true,
                FlagsTransparent = true,
                ParamsCullMode = SpatialMaterial.CullMode.Disabled,
                ParamsSpecularMode = SpatialMaterial.SpecularMode.Disabled,
            }
        };

        AddChild(CollisionShape = new CollisionShape()
        {
            Shape = new BoxShape()
            {
                Extents = new Vector3(
                    ((QuadMesh)MeshInstance.Mesh).Size.x / 2f,
                    ((QuadMesh)MeshInstance.Mesh).Size.y / 2f,
                    0.1f
                    ),
            }
        });
        CollisionShape.AddChild(MeshInstance);

    }
}
