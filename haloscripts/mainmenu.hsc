; Globals
(global short ui_location 0)
(global short currentCharacter -1)

; Externs

; Scripts
(script static void (set_ui_location (short location))
	(set ui_location location)
	(sleep 1)
)

(script static void kill_camera_scripts
	(print "kill camera scripts")
	(kill_active_scripts)
	(if (!= ui_location 0)
		(sleep_forever mainmenu_cam)
	)
	(if (!= ui_location 1)
		(sleep_forever campaign_cam)
	)
	(if (!= ui_location 2)
		(sleep_forever survival_cam)
	)
	(if (!= ui_location 3)
		(sleep_forever custom_cam)
	)
	(if (!= ui_location 4)
		(sleep_forever editor_cam)
	)
	(if (!= ui_location 5)
		(sleep_forever theater_cam)
	)
	(if (!= ui_location 6)
		(sleep_forever settings_cam)
	)
	(if (!= ui_location 7)
		(sleep_forever server_browser_cam)
	)
	(if (!= ui_location 8)
		(sleep_forever matchmaking_cam)
	)
	(if (!= ui_location 9)
		(sleep_forever activities_cam)
	)
)

(script startup void mainmenu
	(print "mainmenu statup script")
	(fade_out 0 0 0 0)
	(set ui_location -1)
	(wake appearance_characters)
	(sleep 8)
	(fade_in 0 0 0 22)
	(camera_control true)
)

(script static void (camera_path_custom (short location))
)

(script static void (camera_path_title (short location))
)

(script static void (camera_path_campaign (short location))
)

(script static void (camera_path_matchmaking (short location))
)

(script static void (camera_path_editor (short location))
)

(script static void (camera_path_theater (short location))
)

(script static void (camera_path_spartans_kiva (short location))
)

(script static void mainmenu_cam
	(print "mainmenu camera")
	(if (< ui_location 6)
		(sleep 10)
	)
	(if (!= ui_location -1)
		(begin
			(set ui_location 0)
			(kill_camera_scripts)
			false
		)
		(set ui_location 0)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "title_01" 0)
		(sleep 0)
		(camera_set "title_02" 1800)
		(sleep 1800)
		(camera_set "title_03" 1800)
		(sleep 1800)
		(camera_set "title_01" 1800)
		(sleep 1800)
	)
	 -1)
	(sleep_forever)
)

(script static void campaign_cam
	(print "campaign camera act 1")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 1)
			(kill_camera_scripts)
			false
		)
		(set ui_location 1)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "campaign_act1_01" 0)
		(sleep 0)
		(camera_set "campaign_act1_02" 1800)
		(sleep 1800)
		(camera_set "campaign_act1_03" 1800)
		(sleep 1800)
		(camera_set "campaign_act1_04" 1800)
		(sleep 1800)
		(camera_set "campaign_act1_01" 1800)
		(sleep 1800)
	)
	 -1)
	(sleep_forever)
)

(script static void matchmaking_cam
	(print "matchmaking camera")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 8)
			(kill_camera_scripts)
			false
		)
		(set ui_location 8)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "matchmaking_01" 0)
		(sleep 10)
		(camera_set "matchmaking_02" 1200)
		(sleep 1200)
		(camera_set "matchmaking_01" 1200)
		(sleep 1200)
	)
	 -1)
	(sleep_forever)
)

(script static void custom_cam
	(print "custom games camera")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 3)
			(kill_camera_scripts)
			false
		)
		(set ui_location 3)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "custom_path_01" 0)
		(sleep 0)
		(camera_set "custom_path_02" 1800)
		(sleep 1800)
		(camera_set "custom_path_01" 1800)
		(sleep 1800)
	)
	 -1)
	(sleep_forever)
)

(script static void editor_cam
	(print "editor camera")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 4)
			(kill_camera_scripts)
			false
		)
		(set ui_location 4)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "editor_path_02" 0)
		(sleep 10)
		(camera_set "editor_path_03" 1200)
		(sleep 1200)
		(camera_set "editor_path_02" 1200)
		(sleep 1200)
	)
	 -1)
	(sleep_forever)
)

(script static void theater_cam
	(print "theater camera")
	(if (!= ui_location -1)
		(begin
			(set ui_location 5)
			(kill_camera_scripts)
			false
		)
		(set ui_location 5)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "theater_cam_01" 0)
		(sleep 10)
		(camera_set "theater_cam_02" 2400)
		(sleep 2400)
		(camera_set "theater_cam_01" 2400)
		(sleep 2400)
	)
	 -1)
	(sleep_forever)
)

(script static void activities_cam
	(print "activities camera")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 9)
			(kill_camera_scripts)
			false
		)
		(set ui_location 9)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "spartans_kiva_00" 0)
		(sleep 0)
		(camera_set "spartans_kiva_01" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_02" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_03" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_00" 1800)
		(sleep 1800)
		false
	)
	 -1)
	(sleep_forever)
)

(script static void survival_cam
	(print "survival camera")
	(sleep 10)
	(if (!= ui_location -1)
		(begin
			(set ui_location 2)
			(kill_camera_scripts)
			false
		)
		(set ui_location 2)
	)
	(sleep_until (begin
		(sleep 0)
		(camera_set "spartans_kiva_00" 0)
		(sleep 0)
		(camera_set "spartans_kiva_01" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_02" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_03" 1800)
		(sleep 1800)
		(camera_set "spartans_kiva_00" 1800)
		(sleep 1800)
		false
	)
	 -1)
	(sleep_forever)
)

(script static void settings_cam
	(print "settings cam")
	(set_ui_location 6)
	(kill_camera_scripts)
	(background_set 0)
	(camera_set "settings_cam" -1)
	(cinematic_light_object "elite" "invalid" appearance_elite "settings_cam")
	(cinematic_light_object "chief" "invalid" appearance_spartan "settings_cam")
)

(script static void leave_settings
	(print "leave settings")
	(background_set -1)
	(mainmenu_cam)
)

(script static void server_browser_cam
	(print "server_browser_cam")
	(set_ui_location 7)
	(kill_camera_scripts)
	(background_set 0)
	(fade_out 0 0 0 0)
	(sleep 5)
	(fade_in 0 0 0 9)
	(sleep_until (begin
		(sleep 0)
		(camera_set "matchmaking_01" 0)
		(sleep 10)
		(camera_set "matchmaking_02" 1200)
		(sleep 1200)
		(camera_set "matchmaking_01" 1200)
		(sleep 1200)
	)
	 -1)
)

(script static void leave_server_browser
	(print "leave server browser")
	(background_set -1)
	(fade_in 0 0 0 9)
	(sound_looping_start "sound\levels\riverworld\halo_ext\halo_ext" "none" 1)
	(mainmenu_cam)
)

(script dormant void appearance_characters
	(print "appearance characters [static script]")
	(pvs_set_object "chief")
	(pvs_set_object "elite")
	(cinematic_light_object "chief" "invalid" appearance_spartan "spartan_light_anchor")
	(cinematic_light_object "elite" "invalid" appearance_elite "elite_light_anchor")
	(sleep 1)
	(custom_animation_loop "chief" "objects\characters\masterchief\masterchief" "combat:unarmed:idle" false)
	(custom_animation_loop "elite" "objects\characters\dervish\dervish" "combat:unarmed:idle" false)
)

(script static void character_changed
	(sleep 1)
	(set currentCharacter (ui_get_player_model_id))
)

(script static void helmet_camera_wide
	(print "helmet camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_helmet_wide" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_helmet_wide" 15)
	)
)

(script static void helmet_camera_standard
	(print "helmet camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_helmet_standard" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_helmet_standard" 15)
	)
)

(script static void leftshoulder_camera_wide
	(print "leftshoulder camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_leftshoulder_wide" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_leftshoulder_wide" 15)
	)
)

(script static void leftshoulder_camera_standard
	(print "leftshoulder camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_leftshoulder_standard" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_leftshoulder_standard" 15)
	)
)

(script static void rightshoulder_camera_wide
	(print "rightshoulder camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_rightshoulder_wide" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_rightshoulder_wide" 15)
	)
)

(script static void rightshoulder_camera_standard
	(print "rightshoulder camera")
	(if (= currentCharacter 0)
		(camera_set "spartan_rightshoulder_standard" 15)
	)
	(if (= currentCharacter 1)
		(camera_set "elite_rightshoulder_standard" 15)
	)
)

(script static void exit_subcamera
	(print "exit subcamera")
	(camera_set "settings_cam" 15)
)