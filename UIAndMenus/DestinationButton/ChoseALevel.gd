extends Button

var mainMenu

func _ready():
	mainMenu = get_parent().get_parent()
	pass
	
func _pressed():
	
	mainMenu.CharacterChosen()
