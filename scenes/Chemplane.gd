extends Spatial


# Declare member variables here. Examples:
var route
var target = 0
var goose_drop_timer = randf() * 3 + 1
var goose_drop_batch = 20
var drop_goose = preload("res://scenes/DropGoose.tscn")
var velocity = Vector3()
const speed = 10.0


# Called when the node enters the scene tree for the first time.
func _ready():
    route = [Vector3(randf()*40+8,20,0).rotated(Vector3.UP,randf())]
    for i in range(randi()%5+10):
        var next = route[-1]
        next.y = 0
        next *= randf() * 0.6 + 0.7
        
        next = next.rotated(Vector3.UP,randf())
        next.y = 20
        route.append(next)
    pass # Replace with function body.

func _physics_process(delta):
    velocity += ((translation.direction_to(route[target]) * speed)
                 - velocity) * delta / 2
    translation+=velocity  * delta
    transform = transform.looking_at(translation + velocity,Vector3.UP)
    if translation.distance_to(route[target]) < 10:
        target+=1;
        target = target % len(route)
        
    goose_drop_timer -= delta
    if goose_drop_timer <= 0:
        goose_drop_batch -= 1
        if goose_drop_batch == 0:
            goose_drop_timer = 5
            goose_drop_batch = 20
        else:
            goose_drop_timer = randf()*0.2 + 0.05
        var new_goose = drop_goose.instance()
        new_goose.translation = translation
        # new_goose.add_central_force(transform.xform(Vector3.BACK)*100)
        new_goose.add_torque(Vector3(randf()*100,randf()*100,randf()*100))
        get_parent().add_child(new_goose)
