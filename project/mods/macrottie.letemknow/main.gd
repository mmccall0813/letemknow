extends Node

var api_url = "http://localhost:10767/api/v1/playback/"

onready var _status = preload("res://mods/macrottie.letemknow/Scenes/StatusFinder/statusfinder.tscn").instance()
var _local_player_id = -1

const ID = "macrottie.letemknow"
const ModVersion = "1.0.0"
func _ready():
	Network.connect("_instance_actor", self, "_instance_actor")
	
	self.add_child(_status)
	
	_status.connect("_status_change", self, "_send_status_change")
	
	_check_options()

func _instance_actor(dict, sender = -1):
	if dict.creator_id == Network.STEAM_ID && dict.actor_type == "player":
		_local_player_id = dict.actor_id
		print(dict.actor_id, " should be our local player id...")

func _send_status_change():
	print(_status.song)
	print(_status.artist)
	if Network.STEAM_LOBBY_ID > 0 && _local_player_id != -1:
		print("sending out packet")
		Network._send_P2P_Packet({"type": "update_song", "song": _status.song, "artist": _status.artist})
		#for actor in get_tree().get_nodes_in_group("actor"):
		#	if actor.actor_id == _local_player_id:
		#		actor._update_nowplaying(_status.song, _status.artist)

# borrowed (tm) from Lure :3
func _check_options():
	var file = File.new()
	print("letemknow searching for gdweave options json")
	if file.open(_get_gdweave_dir().plus_file("/configs/macrottie.letemknow.json"),File.READ) == OK:
		var p = JSON.parse(file.get_as_text())
		file.close()
		var result = p.result
		if typeof(result) == TYPE_DICTIONARY:
			print("options found...")
			_status.lastfm = result.UseLastFm
			_status.fm_user = result.LastFmUsername
			_status.fm_api_key = result.LastFmApikey
			_status.startTimer()
		else:
			print("error reading options... falling back to cider...")
			_status.startTimer()
	else:
		print("couldnt find options... falling back to cider...")
		_status.startTimer()

# borrowed (tm) from TackleBox
func _get_gdweave_dir() -> String:
	var file := File.new()
	var game_directory := OS.get_executable_path().get_base_dir()
	var default_directory := game_directory.plus_file("GDWeave")
	var folder_override: String
	var final_directory: String
	
	for argument in OS.get_cmdline_args():
		if argument.begins_with("--gdweave-folder-override="):
			folder_override = argument.trim_prefix("--gdweave-folder-override=").replace("\\", "/")
	
	if folder_override:
		var relative_path := game_directory.plus_file(folder_override)
		var is_relative := not ":" in relative_path and file.file_exists(relative_path)
		
		final_directory = relative_path if is_relative else folder_override
	else:
		final_directory = default_directory
	
	return final_directory
