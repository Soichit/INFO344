<?php 

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

?>