# BDOMarketplace

## APIBackend [C#]
A class libary which acts as a wrapper for the MySQL connection and querys 

## PacketSniffer [C#]
A sniffer that listen for all of the packets incoming from Black Desert Online and filters and pulls the marketplace transactions to the web api

## WebAPI [C#, ASP.Net]
A ASP.Net web api which both recieves the marketplace transactions and stores them to the MySQL database but also serves the data

## WebCrawler [JS, Node]
A simple web scrapper used to pull the items information off of the wiki's database and put them into the MySQL database for caching

## Website [HTML, CSS, JS]
A demo front-end website that consumes the web api and display's realtime data and graphs
