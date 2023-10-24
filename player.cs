using Godot;

public partial class player : CharacterBody3D
{
    const float speed = 5f;
    const float gravity = 30f;
    const float jumpForce = 10f;
    const float acceleration = 0.5f;
    const float sensitivity = 0.01f;
    const float sprint = 10f;

    public Node3D head;
    public Camera3D cam;

    private Vector3 velocity = Vector3.Zero;

    public override void _Ready()
    {
        head = GetNode<Node3D>("Head");
        cam = GetNode<Camera3D>("Head/Camera3d");

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
            head.RotateY(-mouseMotion.Relative.X * sensitivity);
            cam.RotateX(-mouseMotion.Relative.Y * sensitivity);

            Vector3 camRot = cam.Rotation;
            camRot.X = Mathf.Clamp(camRot.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            cam.Rotation = camRot;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;
            if (Input.IsActionPressed("run"))
            {
                velocity.X = direction.X * sprint;
                velocity.Z = direction.Z * sprint;
                GD.Print("Running");
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, acceleration);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, acceleration);
        }

        if (!IsOnFloor())
        {
            velocity.Y -= gravity * (float)delta;
        }

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.Y = jumpForce;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
