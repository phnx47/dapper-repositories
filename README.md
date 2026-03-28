# MicroOrm.Dapper.Repositories

[![ci](https://img.shields.io/github/actions/workflow/status/phnx47/dapper-repositories/ci.yml?branch=main&label=ci&logo=github%20actions&logoColor=white&style=flat-square)](https://github.com/phnx47/dapper-repositories/actions/workflows/ci.yml)
[![nuget](https://img.shields.io/nuget/v/MicroOrm.Dapper.Repositories?logo=nuget&style=flat-square)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)
[![nuget](https://img.shields.io/nuget/dt/MicroOrm.Dapper.Repositories?logo=nuget&style=flat-square)](https://www.nuget.org/packages/MicroOrm.Dapper.Repositories)
[![codecov](https://img.shields.io/codecov/c/github/phnx47/dapper-repositories?logo=codecov&style=flat-square&token=wR4U6i4vhk)](https://codecov.io/gh/phnx47/dapper-repositories)
[![license](https://img.shields.io/github/license/phnx47/dapper-repositories?style=flat-square)](https://github.com/phnx47/dapper-repositories/blob/main/LICENSE)


## Description

If you like your code to run fast, you probably know about Micro ORMs. They are simple and one of their main goals is to be the fastest execution of your SQL sentences in your data repository. For some Micro ORMs you need to write your own SQL sentences and this is the case of the most popular Micro ORM [Dapper](https://github.com/DapperLib/Dapper).

This library abstracts the generation of SQL for CRUD operations based on each C# POCO class metadata. The SQL Generator is a generic component that builds all CRUD sentences for a POCO class based on its definition, with the possibility to override the SQL generator and the way it builds each sentence.

The library is designed to help with common, simple queries so you don't have to write repetitive SQL by hand. It does not try to cover every possible scenario - for complex queries you should write SQL directly using [Dapper](https://github.com/DapperLib/Dapper).

Read the full documentation at [Learn Dapper](https://www.learndapper.com/extensions/microorm-dapper-repositories).

## Sponsors

[Dapper Plus](https://dapper-plus.net/?utm_source=phnx47&utm_medium=MicroOrm.Dapper.Repositories) and [Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=phnx47&utm_medium=MicroOrm.Dapper.Repositories) are major sponsors and are proud to contribute to the development of [MicroOrm.Dapper.Repositories](https://github.com/phnx47/dapper-repositories)

[![Dapper Plus](https://raw.githubusercontent.com/phnx47/dapper-repositories/main/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=phnx47&utm_medium=MicroOrm.Dapper.Repositories)

[![Entity Framework Extensions](https://raw.githubusercontent.com/phnx47/dapper-repositories/main/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=phnx47&utm_medium=MicroOrm.Dapper.Repositories)

## License

Inspired by [Diego García's Dapper.DataRepositories](https://github.com/ElNinjaGaiden/Dapper.DataRepositories).

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).
