$("#refresh").click(function() {
    UpdateRecentList();
});

setInterval(function(){
    if($(".ui.checkbox").checkbox("is checked")) {
        UpdateRecentList();
    }
}, 1000);
function CreateRecentItem(DateTime, itemPrice, imgsrc, itemName, amount, color, iditem, hidden)
{
    var items = $(iditem);
    var newItem = $("<div class='item " + hidden + "'>"                                                                +
            "<div class='ui inverted raised segment' style='width:100%;height:70px;'>"                  +
                "<div class='ui container' style='width:75%;float:left'>"                               +
                    "<img style='display: inline;width: 40px !important;height: 40px !important;' class='ui image' src='" + imgsrc + "'>"              +
                    "<span style='padding-left: 5px'> <div style='color: " + color + ";display: inline;font-size:19;'>" + itemName + "</div> <div style='display:inline;font-size: 12;'>" + amount + "</div></span>"                           +
               "</div>"                                                                                 +
                "<div class='ui container' style='width:25%;float:right;text-align: right'>"            +
                    "<span><i class='yellow dollar sign icon'></i>" + itemPrice +"</span><div></div>"   +
                    "<span><i class='clock icon'></i>" + DateTime +"</span>"                            + 
                "</div>"                                                                                +
            "</div>"                                                                                    +
        "</div>");
    items.append(newItem.clone());
}


var accessory = ["Witch's Earring", "Ogre Ring", "Blue Coral Ring", "Manos Ruby Necklace", "Ring of Crescent Guardian", "Ring of Cadry Guardian",
                 "Basilisk's Belt", "Laytenn's Power Stone", "Serap's Necklace", "Mark of Shadow", "Belt of Shultz the Gladiator", "Forest Ronaros Ring",
                 "Orkinrad's Belt", "Red Coral Earring", "Tungrad Earring"];

var ChartThing = null;

function Search(output)
{
    if(output.length > 3)
    {
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open( "GET", "http://localhost:61073/api/GetItemsByName/" + output, false ); // false for synchronous request
        xmlHttp.send( null );
        var content = JSON.parse(xmlHttp.responseText);

        $('.ui.search')
        .search({
            source : content,
            searchFields   : [
            'name'
            ],
            fullTextSearch: true,
            fields: {
                title : 'name',
                image : 'url',
                url : ''
            },
            onSelect: function(result, response) {

                if($('#recent').hasClass('hidden'))
                {
                    $('#marketplaceItem').transition('fade up');
                    $("#itemListing").empty();
                } else {
                    $('#recent').transition('fade up');
                }


                
                setTimeout(function() {
                    $('#marketplaceItem').transition('fade down');
                    
                    var xmlHttp = new XMLHttpRequest();
                    xmlHttp.open( "GET", "http://localhost:61073/api/GetItem/" + result.ItemID, false ); // false for synchronous request
                    xmlHttp.send( null );
                    var content = JSON.parse(xmlHttp.responseText);
                    var array = [];
                    var hightest = 0;
                    content.forEach(element => {
                        array.push(
                            {
                                x: new Date(element.registerTime),
                                y: element.price
                            }
                        );  

                        if(element.price > hightest)
                        {
                            hightest = element.price;
                        }
                        var itemAmount = "";
                        if(element.amount > 1)
                        {
                            itemAmount = "x" + element.amount.toString();
                        }

                        var enchantment = "";
                        if(element.enchantment > 15) {
                            switch(element.enchantment) {
                                case 16:
                                    enchantment = "PRI";
                                    break;
                                case 17:
                                    enchantment = "DUO";
                                    break;
                                case 18:
                                    enchantment = "TRI";
                                    break;
                                case 19:
                                    enchantment = "TET";    
                                    break;
                                case 20:
                                    enchantment = "PEN";
                                    break;
                            }
                        } else if(element.enchantment != 0) {
                            if(accessory.includes(element.name))
                            {
                                switch(element.enchantment) {
                                    case 1:
                                        enchantment = "PRI";
                                        break;
                                    case 2:
                                        enchantment = "DUO";
                                        break;
                                    case 3:
                                        enchantment = "TRI";
                                        break;
                                    case 4:
                                        enchantment = "TET";    
                                        break;
                                    case 5:
                                        enchantment = "PEN";
                                        break;
                                }
                            } else {
                                enchantment = "+" + element.enchantment.toString();
                            }
                        }
                        CreateRecentItem(element.registerTime, Number(element.price).toLocaleString(), element.url, enchantment + " " + element.name, itemAmount, '', "#itemListing");
                        
                    });
                    if(ChartThing != null)
                    {
                        ChartThing.destroy();
                    }
                    hightest *= 2;
                    Chart.scaleService.updateScaleDefaults('linear', {
                        ticks: {
                            min: 0,
                            max: hightest
                        }
                    });
                    var ctx = document.getElementById("ecodata").getContext('2d');
                    var myChart = new Chart(ctx, {
                        type: 'line',
                        data: {
                            datasets: [{
                                label: 'Price',
                                data: array,
                                borderColor: 'rgb(192, 57, 43)',
                                backgroundColor: 'rgba(231, 76, 60, 0)'
                            }]
                        },
                        options: {
                            scales: {
                                xAxes: [{
                                    type: 'time',
                                    time: {
                                        unit: 'day'
                                    }
                                }],
                                yAxes: [{
                                    ticks: {
                                        beginAtZero: true,
                                        callback: function(value, index, values) {
                                            if(value == 0)
                                            {
                                                return value;
                                            } else if(value > 999999)
                                            {
                                                return Math.round((value/1000000)) + "M";
                                            } else
                                            {
                                                return (value/1000) + "K";
                                            }
                                        }
                                    },
                                    max: hightest
                                }]
                            },
                            tooltips: {
                                callbacks: {
                                    label: function(tooltipItem, data) {
                                        return 'Price: ' + Number(tooltipItem.yLabel).toLocaleString();
                                    }
                                }
                            },
                            maintainAspectRatio: false
                        }
                    });
                    //width: 100%;height: 30%;
                    myChart.canvas.parentNode.style.height = ($("#marketplaceItem").height() * 0.3) + "px";
                    myChart.canvas.parentNode.style.width = ($("#marketplaceItem").width()) + "px";
                    ChartThing = myChart;
                }, 400);
            }
        })
        ;
    }
}


function Init()
{
    UpdateRecentList();
}
var lastItems = null;
function UpdateRecentList()
{
    if($('#recent').hasClass('hidden'))
    {
        return;
    }
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open( "GET", "http://localhost:61073/api/GetItems/7", false ); // false for synchronous request
    xmlHttp.send( null );
    var data = JSON.parse(xmlHttp.responseText);
    if(_.isEqual(data, lastItems))
    {
        return;
    } else {
        lastItems = data;
    }
    $("#recentItems").empty();
    for (i = 0; i < data.length; i++) { 
        var enchantment = "";
        if(data[i].enchantment > 15) {
            switch(data[i].enchantment) {
                case 16:
                    enchantment = "PRI";
                    break;
                case 17:
                    enchantment = "DUO";
                    break;
                case 18:
                    enchantment = "TRI";
                    break;
                case 19:
                    enchantment = "TET";    
                    break;
                case 20:
                    enchantment = "PEN";
                    break;
            }
        } else if(data[i].enchantment != 0) {
            if(accessory.includes(data[i].name))
            {
                switch(data[i].enchantment) {
                    case 1:
                        enchantment = "PRI";
                        break;
                    case 2:
                        enchantment = "DUO";
                        break;
                    case 3:
                        enchantment = "TRI";
                        break;
                    case 4:
                        enchantment = "TET";    
                        break;
                    case 5:
                        enchantment = "PEN";
                        break;
                }
            } else {
                enchantment = "+" + data[i].enchantment.toString();
            }
        }

        var itemAmount = "";
        if(data[i].amount > 1)
        {
            itemAmount = "x" + data[i].amount.toString();
        }

        var hiddent = "";
        if(i == 0)
        {
            hiddent = "transition hidden";
        }
        CreateRecentItem(data[i].registerTime, Number(data[i].price).toLocaleString(), data[i].url, enchantment + " " + data[i].name, itemAmount, data[i].color, "#recentItems", hiddent);
    }
    setTimeout(function() {
        $('#recentItems').children().eq(0).transition('fade down');
    }, 200);
}

function addCommas(nStr)
{
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function Home()
{
    if($('#recent').hasClass('hidden'))
    {
        $('#marketplaceItem').transition('fade up');
        setTimeout(function() {
            $("#itemListing").empty();
            $('#recent').transition('fade down');
        }, 400);      
    }
}