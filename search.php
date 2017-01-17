

<?php
	require_once('player.php');

	// Business logic that retrieves data from RDS based on user input
	if (isset($_POST["title"]) && !empty($_POST["title"])) {

		// Connect to RDS
		$servername = "mydbinstance.cclthmosu4ny.us-west-2.rds.amazonaws.com";
		$username = "info344user";
		$password = "chingensai";

		try {
		    $conn = new PDO("mysql:host=$servername;dbname=import_csv", $username, $password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		    // echo "Connected successfully"; 
		} catch (PDOException $e) {
		    echo "Connection failed: " . $e->getMessage();
		}

		$myArray = [];
		if (!filter_input(INPUT_POST, "title", FILTER_DEFAULT)) {
	 	   echo("<h3>Your search is not valid</h3>");
		}
		$input = filter_input(INPUT_POST, "title", FILTER_DEFAULT);
		//$input = preg_replace('/\s+/', '', $input);

		try {
		    $query = $conn->prepare("SELECT * FROM PLAYERS1 WHERE Name LIKE  :input");
			$query->execute(['input' => '%' . $input . '%']);
			$conn = null;
		} catch (PDOException $e) {
		    if ($e->getCode() == 1062) {
		    	echo "FALSE";
		    // Take some action if there is a key constraint violation, i.e. duplicate name
		    } else {
		        throw $e;
		    }
		}
		

		// check if query returns no results
		if ($query->rowCount() == 0) {
			echo "<h3>No results found</h3>";
		} else {
			$count = 0;
			$query->setFetchMode(PDO::FETCH_CLASS, 'Player');
			foreach($query->fetchAll() as $row) {
				array_push($myArray, $row);
				$count++;
			}
			echo "<h3>" . $count . " results found</h3>";
			// Presentation layer that inserts content into the user view
			foreach($myArray as $player) {
				echo "<div class='clearfix'>";
				//if ($player->getPicture() === "") {
				// 	echo "<img class='playerImage' src=" . "https://upload.wikimedia.org/wikipedia/commons/a/ac/No_image_available.svg"  . " />";
				// } else {
				// 	echo "<img class='playerImage' src=" . $player->getPicture()  . "/>";
				//}
				echo "<div class='contents'>" .
					"<h2>" . $player->getName() . "</h2>" .
					"<h4>Team: " . $player->getTeam() . " | GP: " . $player->getGP() . " | Min: " . $player->getMin() . "</h4><hr />" .
					"<span class='box1'>" .
						"<h5>FG</h5><br />" .
						"<table border='1'><tr><td>M</td><td>A</td><td>PCT</td></tr>" .
						"<tr><td>" . $player->getFG_M() . "</td><td>" . $player->getFG_A() . "</td><td>" . $player->getFG_Pct() . "</td>" .
						"</tr></table>" .
					"</span>" .

					"<span class='box1'>" .
						"<h5>Three PT</h5><br />" .
						"<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>" .
						"<tr><td>" . $player->getThree_PT_M() . "</td><td>" . $player->getThree_PT_A() . "</td><td>" . $player->getThree_PT_Pct() . "</td>" .
						"</tr></table>" .
					"</span>" .

					"<span class='box1'>" .
						"<h5>FT</h5><br />" .
						"<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>" .
						"<tr><td>" . $player->getFT_M() . "</td><td>" . $player->getFT_A() . "</td><td>" . $player->getFT_Pct() . "</td>" .
						"</tr></table>" .
					"</span>" .
					"<span class='box1'>" .
						"<h5>Rebounds</h5><br />" .
						"<table class='right' border='1'><tr><td>Off</td><td>Def</td><td>Tot</td></tr>" .
						"<tr><td>" . $player->getRebounds_Off() . "</td><td>" . $player->getRebounds_Def() . "</td><td>" . $player->getRebounds_Tot() . "</td>" .
						"</tr></table>" .
				    "</span>" .
					"<span class='box1'>" .
						"<h5>Misc</h5><br />" .
						"<table class='right' border='1'><tr><td>Ast</td><td>TO</td><td>Stl</td><td>Blk</td><td>PF</td><td>PPG</td></tr>" .
						"<tr><td>" . $player->getMisc_Ast() . "</td><td>" . $player->getMisc_TO() . "</td><td>" . $player->getMisc_Stl() . "</td>" .
						"<td>" . $player->getMisc_Blk() . "</td><td>" . $player->getMisc_PF() . "</td><td>" . $player->getMisc_PPG() . "</td>" .
						"</tr></table>" .
					"</span>" .
					"</div>" .
				"</div>";
			}
		}
	}
?>





