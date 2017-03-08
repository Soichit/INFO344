<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="WebRole1.Search" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta charset="utf-8" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.1.1.min.js"></script>
    <link rel="stylesheet" type="text/css" href="StyleSheet1.css">
</head>

<body>
    <div id="top">
        <img src="http://i.imgur.com/crtYgrQ.png" />
    </div>
    <br />

    <div id="center">
        <%--<div id="left">--%>
            <input id="input" type="text" autocomplete="off">
            <div id="dropdown"></div>
        <%--</div>
        <button id="submit" type="submit">Search</button>--%>
    </div>
    <br />

    <div id="results"></div>
    <div id="results2"></div>
    <script async src="//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
    <!-- soichi -->
    <ins class="adsbygoogle"
            style="display:block"
            data-ad-client="ca-pub-5666480866743556"
            data-ad-slot="1552775820"
            data-ad-format="auto"></ins>
    <script>
    (adsbygoogle = window.adsbygoogle || []).push({});
    </script>

    
    
    <script>
        $(document).ready(function () {
            var delay = (function(){
                var timer = 0;
                return function(callback, ms){
                    clearTimeout (timer);
                    timer = setTimeout(callback, ms);
                };
            })();

            //AJAX call to retrieve suggestions when keyup is called
            $("#input").keyup(function () {
                delay(function () {
                        $("#dropdown").show();
                        var input = $('#input').val().trim().toLowerCase();
                        var checkedInput = input.replace(/ /g, "_");
                        if (input == "") {
                            $("#dropdown").removeClass("border");
                            $("#dropdown").html("");
                        } else {
                            $("#dropdown").addClass("border");
                        }
                        
                        var url = "QuerySuggest.asmx/SearchTrie";
                        $.ajax({
                            data: JSON.stringify({ input: checkedInput }),
                            dataType: "json",
                            url: url,
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            success: function (result) {
                                //console.log(result.d);
                                if (result.d == "{}" && $('#input').val() != "") {
                                    $("#dropdown").html("Can't find any results.");
                                } else {
                                    $("#dropdown").html("");
                                    $.each(JSON.parse(result.d), function (key, value) {
                                        $("#dropdown").append($("<div class='drop'></div>").html(key).on("click", function() {
                                            key = key.replace(/\'/g, '');
                                            $('#input').val(key);
                                            //console.log(key);
                                            $("#dropdown").hide();
                                            $("#results").html("");
                                            searchQuery(key);
                                            searchResults(key);
                                        }));
                                    })
                                }
                            }
                        });

                        // search on keyup
                        $("#results").html("");
                        if (input != "") {
                            searchQuery(input);
                            searchResults(input);
                        }
                        }, 500 );
            });

            //hide dropdown
            function searchQuery(input) {
                //console.log(input);
                $.ajax({
                    crossDomain: true,
                    contentType: "application/json; charset=utf-8",
                    url: "http://35.165.133.148/part1/search.php",
                    data: { title: input },
                    dataType: "jsonp",
                    success: onDataReceived
                }).fail(function (xhr, status, errorThrown) {
                    //console.log("Status: " + status);
                    //$("#dropdown").hide();
                });

                function onDataReceived(data) {
                    //console.log(data);
                    var display = "<div class='clearfix'><div class='contents'><h2>" + data[0].Name + "</h2><h4>Team: " + data[0].Team + "| GP: " + data[0].GP + " | Min: " + data[0].Min + "</h4><hr />";
                    display += "<span class='box1'>" + "<h5>FG</h5><br />" + "<table border='1'><tr><td>M</td><td>A</td><td>PCT</td></tr>" + "<tr><td>";
                    display += data[0].FG_M + "</td><td>" + data[0].FG_A + "</td><td>" + data[0].FG_Pct + "</td>" + "</tr></table>" + "</span>";
                    display += "<span class='box1'>" + "<h5>Three PT</h5><br />" + "<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>";
                    display +=  "<tr><td>" + data[0].Three_PT_M + "</td><td>" + data[0].Three_PT_A + "</td><td>" + data[0].Three_PT_Pct + "</td>" + "</tr></table>" + "</span>";
                    display += "<span class='box1'>" + "<h5>FT</h5><br />" + "<table class='right' border='1'><tr><td>M</td><td>A</td><td>Pct</td></tr>";
                    display += "<tr><td>" +  data[0].FT_M + "</td><td>" + data[0].FT_A + "</td><td>" + data[0].FT_Pct + "</td>" + "</tr></table>" + "</span>";
                    display += "<span class='box1'>" + "<h5>Rebounds</h5><br />" + "<table class='right' border='1'><tr><td>Off</td><td>Def</td><td>Tot</td></tr>";
                    display += "<tr><td>" + data[0].Rebounds_Off + "</td><td>" + data[0].Rebounds_Def + "</td><td>" + data[0].Rebounds_Tot + "</td>" + "</tr></table>" + "</span>";
                    display += "<span class='box1'>" + "<h5>Misc</h5><br />" + "<table class='right' border='1'><tr><td>Ast</td><td>TO</td><td>Stl</td><td>Blk</td><td>PF</td><td>PPG</td></tr>";
                    display += "<tr><td>" + data[0].Misc_Ast + "</td><td>" + data[0].Misc_TO + "</td><td>" + data[0].Misc_Stl + "</td>";
                    display += "<td>" + data[0].Misc_Blk + "</td><td>" + data[0].Misc_PF + "</td><td>" + data[0].Misc_PPG + "</td>" + "</tr></table>" + "</span>";
                    display += "</div>" + "</div>";
                    $("#results").html(display);
                }
            };



            function searchResults(input) {
                var url = "Admin.asmx/getResults";
                $.ajax({
                    data: JSON.stringify({ input: input }),
                    dataType: "json",
                    url: url,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                        //console.log(result);
                        //console.log(result.d);
                        if (result.d == null) {
                            //console.log("empty");
                        }
                        $("#results2").html("");
                        $.each(JSON.parse(result.d), function (key, value) {
                            //console.log(key);
                            //console.log(value);
                            var value = value.webpage;
                            var date = value.date;
                            var year = date.substring(0, 4);
                            var month = date.substring(5, 7);
                            var day = date.substring(8, 10);
                            
                            var resultsBox = $("<div>").html("<a href='" + value.url + "'><img class='results-img' src='" + value.imgUrl + "'/></a>");
                            var resultsContent = $("<div>", { 'class': 'results-content'});
                            var url = $("<a>").attr("href", value.url);
                            var h3 = $("<h3>", { 'class': 'results-title' }).text(value.title);
                            var p = $("<p>", { 'class': 'results-url' }).text(value.url);
                            if (date != "NA") {
                                date = $("<span>", { 'class': 'results-date' }).text(month + "/" + day + "/" + year + " - ");
                            } else {
                                date = "";
                            }
                            
                            var bodyWords = value.body.split(" ");
                            var inputWords = input.split(" ");
                            //console.log(inputWords);
                            var finalWords = "";
                            inputWords.forEach(function(item) {
                                bodyWords.forEach(function (word) {
                                    if (item.length > 3) {
                                        if (word.toLowerCase().includes(item.toLowerCase())) {
                                            word = "<b>" + word + "</b>";
                                        }
                                    } else {
                                        if (word.toLowerCase() == item.toLowerCase()) {
                                            word = "<b>" + word + "</b>";
                                        }
                                    }
                                    finalWords += word + " ";
                                });
                            });
                            var body = $("<span>", { 'class': 'results-body' }).html(finalWords);

                            resultsContent.append(url.append(h3)).append(p).append(date).append(body);
                            $("#results2").append(resultsBox).append(resultsContent);
                        })
                    }
                }).fail(function (xhr, status, errorThrown) {
                    console.log(errorThrown);
                });
            }
        });
    </script>
</body>
</html>
