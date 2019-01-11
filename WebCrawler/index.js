const rp = require('request-promise');
var $ = require('cheerio');
const ascii = /^[ -~\t\n\r]+$/;
var mysql = require('mysql');
var total = 0;
var con = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "test123"
});
var multiple = 4;
con.connect(function(err) {
    if(err) throw err;
    
    GetDBData(multiple);
    setInterval(function() {
        GetDBData(multiple);
    }, (15 * 1000));
})


function GetDBData(m)
{
    for (i = (1000 * m) - 1000; i < 1000 * m; i++) {
        url = 'https://bddatabase.net/us/item/' + i + '/';
        if(i % 100 == 0)
        {
            console.log(i);
        }
        rp(url)
        .then(function(html){
            const load = $.load(html);
            var jsonData = JSON.parse(load('script').get()[6].children[0].data);
            if(jsonData.name != "" && ascii.test(jsonData.name)) {
                var itemid = jsonData.description.split(' ')[1];
                sql = "INSERT INTO bdomp.item_data (itemid, itemname, itempicture) VALUES('" + itemid + "', " + mysql.escape(jsonData.name) + ", '" + jsonData.image + "');"
                con.query(sql, function(err, result) {
                    if(err) {

                    } else {
                        console.log(total + " " + jsonData.name);
                        total++;
                    }
                })
            }
        })
        .catch(function(err){
            //handle error
        });
    }
    multiple++;
}