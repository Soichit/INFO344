<html>
<head>
   <!-- Link your php/css file -->
   <link rel="stylesheet" href="style2.css" media="screen">
</head>


<?php

	// player class
	class Player {
		public function __construct() {
	    }

		function getName() {
			return $this->Name;
		}
		function getTeam() {
			return $this->Team;
		}
		function getGP() {
			return $this->GP;
		}
		function getMin() {
			return $this->Min;
		}
		function getFG_M() {
			return $this->FG_M;
		}
		function getFg_A() {
			return $this->FG_A;
		}
		function getFG_Pct() {
			return $this->FG_Pct;
		}
		function getThree_PT_M() {
			return $this->Three_PT_M;
		}
		function getThree_PT_A() {
			return $this->Three_PT_A;
		}
		function getThree_PT_Pct() {
			if ($this->Three_PT_Pct == Null) {
				return "N/A";
			} else {
				return $this->Three_PT_Pct;
			}
		}
		function getFT_M() {
			return $this->FT_M;
		}
		function getFT_A() {
			return $this->FT_A;
		}
		function getFT_Pct() {
			return $this->FT_Pct;
		}
		function getRebounds_Off() {
			return $this->Rebounds_Off;
		}
		function getRebounds_Def() {
			return $this->Rebounds_Def;
		}
		function getRebounds_Tot() {
			return $this->Rebounds_Tot;
		}
		function getMisc_Ast() {
			return $this->Misc_Ast;
		}
		function getMisc_TO() {
			return $this->Misc_TO;
		}
		function getMisc_Stl() {
			return $this->Misc_Stl;
		}
		function getMisc_Blk() {
			return $this->Misc_Blk;
		}
		function getMisc_PF() {
			return $this->Misc_PF;
		}
		function getMisc_PPG() {
			return $this->Misc_PPG;
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
			echo "<div class='clearfix'>";
				echo "<h2>" . $player->getName() . "</h2>";
				echo "<h4>Team: " . $player->getTeam() . " | GP: " . $player->getGP() . " | Min: " . $player->getMin() . "</h4><hr />";

				echo "<div class='box1'/>";
					echo "<h5>FG</h5>";
					echo "<table border='1'><tr><td>M</td><td>A</td><td>PCT</td></tr>";
					echo "<tr><td>" . $player->getFG_M() . "</td><td>" . $player->getFG_A() . "</td><td>" . $player->getFG_Pct() . "</td>";
					echo "</tr></table>";

					echo "<h5>Three PT</h5>";
					echo "<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>";
					echo "<tr><td>" . $player->getThree_PT_M() . "</td><td>" . $player->getThree_PT_A() . "</td><td>" . $player->getThree_PT_Pct() . "</td>";
					echo "</tr></table>";
				echo "</div>";

				echo "<div class='box1'/>";
					echo "<h5>FT</h5>";
					echo "<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>";
					echo "<tr><td>" . $player->getFT_M() . "</td><td>" . $player->getFT_A() . "</td><td>" . $player->getFT_Pct() . "</td>";
					echo "</tr></table>";

					echo "<h5>Rebounds</h5>";
					echo "<table class='right' border='1'><tr><td>Off</td><td>Deff</td><td>Tot</td></tr>";
					echo "<tr><td>" . $player->getRebounds_Off() . "</td><td>" . $player->getRebounds_Def() . "</td><td>" . $player->getRebounds_Tot() . "</td>";
					echo "</tr></table>";
				echo "</div>";

				echo "<div class='box1'/>";
					echo "<h5>Misc</h5>";
					echo "<table class='right' border='1'><tr><td>Ast</td><td>TO</td><td>Stl</td><td>Blk</td><td>PF</td><td>PPG</td></tr>";
					echo "<tr><td>" . $player->getMisc_Ast() . "</td><td>" . $player->getMisc_TO() . "</td><td>" . $player->getMisc_Stl() . "</td>";
					echo "<td>" . $player->getMisc_Blk() . "</td><td>" . $player->getMisc_PF() . "</td><td>" . $player->getMisc_PPG() . "</td>";
					echo "</tr></table>";
				echo "</div>";
			echo "</div>";
		}
	}

	$conn = null;
?>






