extends Node

var lastfm = false
var fm_user = ""
var fm_api_key = ""

var song = ""
var artist = ""
var playing = false

signal _status_change

func _ready():
	$HTTPRequest.connect("request_completed", self, "_on_request_completed")
	$Timer.connect("timeout", self, "_on_Timer_timeout")
	
func _on_Timer_timeout():
	print("fetching song...")
	if $HTTPRequest.get_http_client_status() != 0:
		print("STILL PROCESSING THE LAST REQUEST. NEVERMIND.")
		return
	if lastfm:
		$HTTPRequest.request("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user="+ fm_user + "&api_key=" + fm_api_key + "&format=json&limit=1")
	else:
		$HTTPRequest.request("http://127.0.0.1:10767/api/v1/playback/is-playing")

func startTimer():
	$Timer.start()

func _on_request_completed(result, response_code, headers, body):
	if response_code != 200: return
	
	var json = JSON.parse(body.get_string_from_utf8())
	# print(json.result)
	var res = json.result

	# god help me this code is getting to yanderedev levels of conditional nesting
	if lastfm:
		# lastfm
		if "message" in res && typeof(res.message) == TYPE_STRING:
			# something bad happened, log the error message to console and stop the loop
			print("\n\n" + " LAST FM ERROR\n" +res.message + "\n\n") # the newlines are so its easier to spot when debugging
			$Timer.stop()
		elif "recenttracks" in res && typeof(res.recenttracks) == TYPE_DICTIONARY && "track" in res.recenttracks && typeof(res.recenttracks.track) == TYPE_ARRAY:
			var recent = res.recenttracks.track
			if recent.size() > 0:
				var changed = false;
				if "@attr" in recent[0] && "nowplaying" in recent[0]["@attr"] && recent[0]["@attr"]["nowplaying"] == "true":
					changed = (recent[0].name != song || recent[0]["artist"]["#text"] != artist)
					song = recent[0].name
					artist = recent[0]["artist"]["#text"]
				else:
					changed = (song != "" || artist != "")
					song = ""
					artist = ""
				
				if changed:
					print("Now playing: " + song + " by " + artist)
					emit_signal("_status_change")
			
	else:
		# cider
		if json.error == OK:
			if "is_playing" in res && typeof(res.is_playing) == TYPE_BOOL && res.is_playing == true:
				print("requesting now playing")
				$HTTPRequest.request("http://127.0.0.1:10767/api/v1/playback/now-playing")
			elif "is_playing" in res && typeof(res.is_playing) == TYPE_BOOL && res.is_playing == false:
				var changed = ("" != song || "" != artist);
				song = ""
				artist = ""
				if changed:
					print("Clearing song status")
					emit_signal("_status_change")
			
			if "info" in res && "name" in res.info && "artistName" in res.info && typeof(res.info.name) == TYPE_STRING && typeof(res.info.artistName) == TYPE_STRING:
				var changed = (res.info.name != song || res.info.artistName != artist);
				
				song = res.info.name
				artist = res.info.artistName
				
				if changed:
					print("Now playing: " + song + " by " + artist)
					emit_signal("_status_change")
