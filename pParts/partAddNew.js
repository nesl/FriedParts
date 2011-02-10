//=================================================================================
//= Submit Part Error Grid
//=================================================================================
//The Submit New Part Validation Grid -- Errors found and user clicked on one for more detail and focusing
//This function works in two parts because the Javascript has to go to the server to get the data;
//Once data is retrieved the second function is executed automagically
function xGridSubmit_Clicked_Part1(){
    xGridSubmit.GetRowValues(xGridSubmit.GetFocusedRowIndex(), 'Step;Code;', xGridSubmit_Clicked_Part2);
}
function xGridSubmit_Clicked_Part2(Values){
    xTabPages.SetActiveTabIndex(Values[0]); //Move to the correct tab page
}

//=================================================================================
//= Part Value Functions
//=================================================================================
function IsNumeric(num){
    var numericExpression = /^[-\+]?[0-9]*\.?[0-9]+([eE][-\+]?[0-9]+)?$/;
    if (num.match(numericExpression) != null) return true;
    else return false;
}

function pvIsBetween(A, LowerLimit, UpperLimit){
    if ((A < LowerLimit) || (A >= UpperLimit)) return false;
    else return true;
}
    
function pvFormatUnits(unitsIn){
    //Capitalize the first letter of the unit's name
    switch(unitsIn.length){
        case 0:
            return "";
            break;
        case 1:
            return unitsIn.toUpperCase();
            break;
        default:
            return unitsIn.substr(0,1).toUpperCase() + unitsIn.substring(1,unitsIn.length);
    }
} //function pvFormatUnits(unitsIn){

function pvDisplayValue(textIn, unitsIn){
    var a = pvValidNumeric(textIn);
    if (IsNumeric(String(a))) {
        return pvFormatNumber(a, true) + pvFormatUnits(unitsIn);
    }
    else return a;
} //function pvDisplayValue(textIn){
    
function pvValidNumeric(textIn){
    var numValue; //The return string
    //Sanity checks
        if (textIn.length == 0) return "";
        if (textIn.length > 255) return "ERROR: TOO LONG!";
    //Pre-Process
        textIn = textIn.replace(" ", ""); //Remove whitespace -- e.g. " 3.4 pF " --> "3.4pF"
        textIn = textIn.toUpperCase(); //Uppercase everything for comparison simplicity

    //Work backwards
        var i = textIn.length;
        while (i != 0 && !IsNumeric(textIn.substr(0, i))){
            i = i - 1;
        }

    //Calculate result
    if (i == 0) return "Not valid!"; //No numeric part found
    else {
        //Numeric part found from indexes 0 to i-1
        //Index i is the start of the non-numeric part
        var SImultiplier = pvValidPrefix(textIn.substr(i, textIn.length - i));
        if (SImultiplier != 0){
            numValue = textIn.substr(0, i) * SImultiplier;
            return numValue;
        }
        else return "Not valid!"; //No valid SI Prefix found
    }
}
   
/* 
//Checks the string to see if it is a valid pSpice SI prefix -- e.g. Meg, k, m, u, n, p, f
//Empty string is valid as an x1 multiplier
//returns the numeric multiplier to numValue (pass by reference) if textIn is valid
//NOTE: Pass ONLY THE PREFIX or you will get a false negative "42k" = false, "k" = true
*/
function pvValidPrefix(textIn){
    var numValue; //output string
    //Sanity checks
        if (textIn.length == 0) return 1; //x1 multiplier implied by empty string
        if (textIn.length > 255) return "Numeric text value is too long! Must be less than 255 characters! Try using scientific notation!";
    //Process
        textIn = textIn.replace(" ", ""); //Remove internal whitespace -- e.g. "3.4 pF" --> "3.4pF"
        textIn = textIn.toUpperCase(); //Uppercase everything for simplicity
    //Select from set
    switch(String(textIn) ){
        case "GIG":
            return 1000000000.0;
            break;
        case "MEG":
            return 1000000.0;
            break;
        case "K":
            return 1000.0;
            break;
        case "M":
            return 0.001;
            break;
        case "U":
            return 0.000001;
            break;
        case "N":
            return 0.000000001;
            break;
        case "P":
            return 0.000000000001;
            break;
        case "F":
            return 0.000000000000001;
            break;
        default:
            return 0; //ERROR!
    } //switch
} //function pvValidPrefix(textIn){

//Formats the numbers for human-readable display
function Format(strIn){
    //is there a decimal point?
    var a = parseFloat(strIn);
    return a.toFixed(3);
}

//Converts a double into a human readable expression
function pvFormatNumber(numIn, verbose){
    var Multiplier

    //Fempto numbers
    Multiplier = 1e-17;
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / (Multiplier * 100)) + "f";
        else
            return "" + Format(numIn / (Multiplier * 100)) + " fempto";
    }

    //Pico numbers
    Multiplier = 0.00000000000001; //1e-14
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / (Multiplier * 100)) + "p";
        else
            return "" + Format(numIn / (Multiplier * 100)) + " pico";
    }

    //Nano numbers
    Multiplier = 0.00000000001 //1e-11
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / (Multiplier * 100)) + "n";
        else
            return "" + Format(numIn / (Multiplier * 100)) + " nano";
    }

    //Micro numbers
    Multiplier = 0.00000001 //1e-8
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / (Multiplier * 100)) + "u";
        else
            return "" + Format(numIn / (Multiplier * 100)) + " micro";
    }

    //Milli numbers
    Multiplier = 0.00001 //1e-5
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / (Multiplier * 100)) + "m";
        else
            return "" + Format(numIn / (Multiplier * 100)) + " milli";
    }

    //Raw numbers
    if (pvIsBetween(numIn, 0, 1000)) {
        if (!verbose) 
            return Format(numIn);
        else
            return numIn;
    }

    //Kilo numbers
    Multiplier = 1000.0
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose)
            return Format(numIn / Multiplier) + "k";
        else
            return "" + Format(numIn / Multiplier) + " kilo";
    }

    //Mega numbers
    Multiplier = 1000000.0
    if (pvIsBetween(numIn, Multiplier, Multiplier * 1000.0)) {
        if (!verbose) 
            return Format(numIn / Multiplier) + "Meg";
        else
            return "" + Format(numIn / Multiplier) + " Mega";
    }

    //Giga numbers
    Multiplier = 1000000000.0
    if (numIn >= Multiplier) {
        if (!verbose) 
            return Format(numIn / Multiplier) + "Gig";
        else
            return "" + Format(numIn / Multiplier) + " Giga";
    }
}

