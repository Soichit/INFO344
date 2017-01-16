<html>
<head>
   <!-- Link your php/css file -->
   <link rel="stylesheet" href="style2a.css" media="screen">
</head>


<?php
	// user input form
	echo "<header>" .
			"<img id='ball' src='http://pngimg.com/upload/basketball_PNG1101.png' />" .
		"</header>" .
		
		"<form action='search.php' method='post'>" .
			"<label>" .
				"<img id='logo' src='https://goodlogo.com/images/logos/national_basketball_association_nba_logo_2414.gif' />" .
				"<input type='text' name='name' autocomplete='off' placeholder='Enter Player Name' required minlength='3' />" .
				"<input id='submit' type='submit' value='Search' />" .
			"</label>" .
		"</form>";

	// player class
	class Player {
		public function __construct() {
	    }
		function getName() {
			if ($this->Name == Null) {
				return "N/A";
			} else {
				return $this->Name;
			}
		}
		function getTeam() {
			if ($this->Team == Null) {
				return "N/A";
			} else {
				return $this->Team;
			}
		}
		function getGP() {
			if ($this->GP == Null) {
				return "N/A";
			} else {
				return $this->GP;
			}
		}
		function getMin() {
			if ($this->Min == Null) {
				return "N/A";
			} else {
				return $this->Min;
			}
		}
		function getFG_M() {
			if ($this->FG_M == Null) {
				return "N/A";
			} else {
				return $this->FG_M;
			}
		}
		function getFg_A() {
			if ($this->FG_A == Null) {
				return "N/A";
			} else {
				return $this->FG_A;
			}
		}
		function getFG_Pct() {
			if ($this->FG_Pct == Null) {
				return "N/A";
			} else {
				return $this->FG_Pct;
			}
		}
		function getThree_PT_M() {
			if ($this->Three_PT_M == Null) {
				return "N/A";
			} else {
				return $this->Three_PT_M;
			}
		}
		function getThree_PT_A() {
			if ($this->Three_PT_A == Null) {
				return "N/A";
			} else {
				return $this->Three_PT_A;
			}
		}
		function getThree_PT_Pct() {
			if ($this->Three_PT_Pct == Null) {
				return "N/A";
			} else {
				return $this->Three_PT_Pct;
			}
		}
		function getFT_M() {
			if ($this->FT_M == Null) {
				return "N/A";
			} else {
				return $this->FT_M;
			}
		}
		function getFT_A() {
			if ($this->FT_A == Null) {
				return "N/A";
			} else {
				return $this->FT_A;
			}
		}
		function getFT_Pct() {
			if ($this->FT_Pct == Null) {
				return "N/A";
			} else {
				return $this->FT_Pct;
			}
		}
		function getRebounds_Off() {
			if ($this->Rebounds_Off == Null) {
				return "N/A";
			} else {
				return $this->Rebounds_Off;
			}
		}
		function getRebounds_Def() {
			if ($this->Rebounds_Def == Null) {
				return "N/A";
			} else {
				return $this->Rebounds_Def;
			}
		}
		function getRebounds_Tot() {
			if ($this->Rebounds_Tot == Null) {
				return "N/A";
			} else {
				return $this->Rebounds_Tot;
			}
		}
		function getMisc_Ast() {
			if ($this->Misc_Ast == Null) {
				return "N/A";
			} else {
				return $this->Misc_Ast;
			}
		}
		function getMisc_TO() {
			if ($this->Misc_TO == Null) {
				return "N/A";
			} else {
				return $this->Misc_TO;
			}
		}
		function getMisc_Stl() {
			if ($this->Misc_Stl == Null) {
				return "N/A";
			} else {
				return $this->Misc_Stl;
			}
		}
		function getMisc_Blk() {
			if ($this->Misc_Blk == Null) {
				return "N/A";
			} else {
				return $this->Misc_Blk;
			}
		}
		function getMisc_PF() {
			if ($this->Misc_PF == Null) {
				return "N/A";
			} else {
				return $this->Misc_PF;
			}
		}
		function getMisc_PPG() {
			if ($this->Misc_PPG == Null) {
				return "N/A";
			} else {
				return $this->Misc_PPG;
			}
		}
	}

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


	// Business logic that retrieves data from RDS based on user input
	$myArray = [];
	if (!filter_input(INPUT_POST, "name", FILTER_DEFAULT)) {
	    echo("<h3>Your search is not valid</h3>");
	}
	$input = filter_input(INPUT_POST, "name", FILTER_DEFAULT);

	try {
	    $query = $conn->prepare("SELECT * FROM PLAYERS1 WHERE Name LIKE  :input");
		$query->execute(['input' => '%' . $input . '%']);
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
		echo "<h3>Sorry we couldn't find the player you're looking for</h3>";
	} else {
		$query->setFetchMode(PDO::FETCH_CLASS, 'Player');
		foreach($query->fetchAll() as $row) {
			array_push($myArray, $row);
		}

		// Presentation layer that inserts content into the user view
		foreach($myArray as $player) {
			echo "<div class='clearfix'>" .
				"<h2>" . $player->getName() . "</h2>" .
				"<h4>Team: " . $player->getTeam() . " | GP: " . $player->getGP() . " | Min: " . $player->getMin() . "</h4><hr />" .

				"<div class='box1'/>" .
					"<h5>FG</h5>" .
					"<table border='1'><tr><td>M</td><td>A</td><td>PCT</td></tr>" .
					"<tr><td>" . $player->getFG_M() . "</td><td>" . $player->getFG_A() . "</td><td>" . $player->getFG_Pct() . "</td>" .
					"</tr></table>" .

					"<h5>Three PT</h5>" .
					"<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>" .
					"<tr><td>" . $player->getThree_PT_M() . "</td><td>" . $player->getThree_PT_A() . "</td><td>" . $player->getThree_PT_Pct() . "</td>" .
					"</tr></table>" .
				"</div>" .

				"<div class='box1'/>" .
					"<h5>FT</h5>" .
					"<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>" .
					"<tr><td>" . $player->getFT_M() . "</td><td>" . $player->getFT_A() . "</td><td>" . $player->getFT_Pct() . "</td>" .
					"</tr></table>" .

					"<h5>Rebounds</h5>" .
					"<table class='right' border='1'><tr><td>Off</td><td>Def</td><td>Tot</td></tr>" .
					"<tr><td>" . $player->getRebounds_Off() . "</td><td>" . $player->getRebounds_Def() . "</td><td>" . $player->getRebounds_Tot() . "</td>" .
					"</tr></table>" .
			    "</div>" .

				"<div class='box1'/>" .
					"<h5>Misc</h5>" .
					"<table class='right' border='1'><tr><td>Ast</td><td>TO</td><td>Stl</td><td>Blk</td><td>PF</td><td>PPG</td></tr>" .
					"<tr><td>" . $player->getMisc_Ast() . "</td><td>" . $player->getMisc_TO() . "</td><td>" . $player->getMisc_Stl() . "</td>" .
					"<td>" . $player->getMisc_Blk() . "</td><td>" . $player->getMisc_PF() . "</td><td>" . $player->getMisc_PPG() . "</td>" .
					"</tr></table>" .
				"</div>" .
			"</div>";
		}
	}

	$conn = null;
?>






