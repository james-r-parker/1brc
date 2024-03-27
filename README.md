# 1️⃣🐝🏎️ The One Billion Row Challenge

The One Billion Row Challenge (1BRC) is a fun exploration of how far modern dotnet can be pushed for aggregating one billion rows from a text file.
Grab all your (virtual) threads, reach out to SIMD, optimize your GC, or pull any other trick, and create the fastest implementation for solving this task!

The text file contains temperature values for a range of weather stations.
Each row is one measurement in the format `<string: station name>;<double: measurement>`, with the measurement value having exactly one fractional digit.
The following shows ten rows as an example:

```
Hamburg;12.0
Bulawayo;8.9
Palembang;38.8
St. John's;15.2
Cracow;12.6
Bridgetown;26.9
Istanbul;6.2
Roseau;34.4
Conakry;31.2
Istanbul;23.0
```

The task is to write a dotnet program which reads the file, calculates the min, mean, and max temperature value per weather station, and emits the results on stdout like this
(i.e. sorted alphabetically by station name, and the result values per station in the format `<min>/<mean>/<max>`, rounded to one fractional digit):

```
{Abha=-23.0/18.0/59.2, Abidjan=-16.2/26.0/67.3, Abéché=-10.0/29.4/69.0, Accra=-10.1/26.4/66.4, Addis Ababa=-23.7/16.0/67.0, Adelaide=-27.8/17.3/58.5, ...}
```

### Dataset
```bash
git clone https://huggingface.co/datasets/nietras/1brc.data
```

_Q: Why_ 1️⃣🐝🏎️ _?_\
A: It's the abbreviation of the project name: **One** **B**illion **R**ow **C**hallenge.

## Results

* AMD Ryzen 9 7950X3D 16-Core Processor, 64GB RAM (5600MHz),
* Windows 11 (10.0.22631), .NET 8.0.201
* Score: 1.28 Seconds

## 1BRC on the Web

A list of external resources such as blog posts and videos, discussing 1BRC and specific implementations:

- [The One Billion Row Challenge Shows That Java Can Process a One Billion Rows File in Two Seconds ](https://www.infoq.com/news/2024/01/1brc-fast-java-processing), by Olimpiu Pop (interview)
- [Cliff Click discussing his 1BRC solution on the Coffee Compiler Club](https://www.youtube.com/watch?v=NJNIbgV6j-Y) (video)
- [1️⃣🐝🏎️🦆 (1BRC in SQL with DuckDB)](https://rmoff.net/2024/01/03/1%EF%B8%8F%E2%83%A3%EF%B8%8F-1brc-in-sql-with-duckdb/), by Robin Moffatt (blog post)
- [1 billion rows challenge in PostgreSQL and ClickHouse](https://ftisiot.net/posts/1brows/), by Francesco Tisiot (blog post)
- [The One Billion Row Challenge with Snowflake](https://medium.com/snowflake/the-one-billion-row-challenge-with-snowflake-f612ae76dbd5), by Sean Falconer (blog post)
- [One billion row challenge using base R](https://www.r-bloggers.com/2024/01/one-billion-row-challenge-using-base-r/), by David Schoch (blog post)
- [1 Billion Row Challenge with Apache Pinot](https://hubertdulay.substack.com/p/1-billion-row-challenge-in-apache), by Hubert Dulay (blog post)
- [One Billion Row Challenge In C](https://www.dannyvankooten.com/blog/2024/1brc/), by Danny Van Kooten (blog post)
- [One Billion Row Challenge in Racket](https://defn.io/2024/01/10/one-billion-row-challenge-in-racket/), by Bogdan Popa (blog post)
- [The One Billion Row Challenge - .NET Edition](https://dev.to/mergeconflict/392-the-one-billion-row-challenge-net-edition), by Frank A. Krueger (podcast)
- [One Billion Row Challenge](https://curiouscoding.nl/posts/1brc/), by Ragnar Groot Koerkamp (blog post)
- [ClickHouse and The One Billion Row Challenge](https://clickhouse.com/blog/clickhouse-one-billion-row-challenge), by Dale McDiarmid (blog post)
- [One Billion Row Challenge & Azure Data Explorer](https://nielsberglund.com/post/2024-01-28-one-billion-row-challenge--azure-data-explorer/), by Niels Berglund (blog post)
- [One Billion Row Challenge - view from sidelines](https://www.chashnikov.dev/post/one-billion-row-challenge-view-from-sidelines), by Leo Chashnikov (blog post)
- [1 billion row challenge in SQL and Oracle Database](https://geraldonit.com/2024/01/31/1-billion-row-challenge-in-sql-and-oracle-database/), by Gerald Venzl (blog post)
- [One Billion Row Challenge: Learned So Far](https://gamlor.info/posts-output/2024-01-12-one-billion-row-challenge/en/), by Roman Stoffel (blog post)
- [One Billion Row Challenge in Racket](https://defn.io/2024/01/10/one-billion-row-challenge-in-racket/), by Bogdan Popa (blog post)
- [The 1 Billion row challenge with Singlestore](https://medium.com/@testily/the-1-billion-row-challenge-with-singlestore-224ce97e451f), by Anna Semjen (blog post)
- [1BRC in .NET among fastest on Linux: My Optimization Journey](https://hotforknowledge.com/2024/01/13/1brc-in-dotnet-among-fastest-on-linux-my-optimization-journey/), by Victor Baybekov (blog post)
- [One Billion Rows – Gerald’s Challenge](https://connor-mcdonald.com/2024/02/03/one-billion-rows-geralds-challenge/), by Connor McDonald (blog post)
- [Reading a file insanely fast in Java](https://rmannibucau.metawerx.net/reading-a-file-insanely-fast-in-java.html), by Romain Manni-Bucau (blog post)
- [#1BRC Timeline](https://tivrfoa.github.io/java/benchmark/performance/2024/02/05/1BRC-Timeline.html), by tivrfoa (blog post)
- [1BRC - What a Journey](https://www.esolutions.tech/1brc-what-a-journey), by Marius Staicu (blog post)
- [One Billion Rows Challenge in Golang](https://www.bytesizego.com/blog/one-billion-row-challenge-go), by Shraddha Agrawal (blog post)
- [The Billion Row Challenge (1BRC) - Step-by-step from 71s to 1.7s](https://questdb.io/blog/billion-row-challenge-step-by-step/) by Marko Topolnik (blog post)
