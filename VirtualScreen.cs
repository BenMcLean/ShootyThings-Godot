﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class VirtualScreen : StaticBody
{
    public const float Height = (float)2.92608;
    public const float PixelWidth = 0.0381f;
    public CollisionShape CollisionShape { get; set; }
    public MeshInstance MeshInstance { get; set; }
    private Viewport Viewport;
    private ColorRect Target;

    public VirtualScreen()
    {
        AddChild(Viewport = new Viewport()
        {
            Size = new Vector2(64, 64),
            Disable3d = true,
            RenderTargetClearMode = Viewport.ClearMode.OnlyNextFrame,
            RenderTargetVFlip = true,
        });
        Viewport.AddChild(new Sprite()
        {
            Centered = false,
            Texture = (Texture)GD.Load("res://icon.png"),
            Scale = new Vector2(2f / 3f, 2f / 3f),
        });
        Viewport.AddChild(Target = new ColorRect()
        {
            Color = Color.Color8(0, 0, 255, 255),
            RectSize = new Vector2(2, 2),
        });
        MeshInstance = new MeshInstance()
        {
            Mesh = new QuadMesh()
            {
                Size = new Vector2(Height, Height),
            },
            MaterialOverride = new SpatialMaterial()
            {
                AlbedoTexture = Viewport.GetTexture(),
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
            Name = "The Virtual Screen!",
            Shape = new BoxShape()
            {
                Extents = new Vector3(
                    ((QuadMesh)MeshInstance.Mesh).Size.x / 2f,
                    ((QuadMesh)MeshInstance.Mesh).Size.y / 2f,
                    PixelWidth
                    ),
            }
        });
        CollisionShape.AddChild(MeshInstance);

        TargetPosition = new Vector2(Height / 2f, Height / 2f);
    }

    public Vector2 TargetPosition
    {
        set => Target.RectPosition = new Vector2(value.x / Height * Viewport.Size.x, value.y / Height * Viewport.Size.y);
        get => new Vector2(Target.RectPosition.x / Viewport.Size.x * Height, Target.RectPosition.y / Viewport.Size.y * Height);
    }
}
