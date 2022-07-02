extends RigidBody

var landed = false
var countdown = 3.0
var true_goose = preload("res://scenes/units/goose/Goose.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
    connect("body_entered",self,"splash");
    pass # Replace with function body.

func splash(_node):
    $Hit.global_transform.basis = Basis.IDENTITY
    $Hit.emitting = true

func _process(delta):
    if not landed and linear_velocity.length()< 0.3 and translation.y < 2:
        landed = true
    elif landed:
        add_force(Vector3.UP*0.01, transform.xform(Vector3(0,1000,0)))
        angular_velocity *= 1-delta
        countdown -= delta
        if countdown <= 0:
            var g = true_goose.instance()
            get_parent().add_child(g)
            g.translation = translation
            g.translation.y = 0
            g.rotation_degrees.y = rotation_degrees.y
            g.add_to_group("Geese")
            get_node("../Container/HBoxContainer/SpinBox").value+=1
            queue_free()

