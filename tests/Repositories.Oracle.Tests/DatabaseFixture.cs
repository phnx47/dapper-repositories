using System;
using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Oracle.ManagedDataAccess.Client;
using Repositories.Base;

namespace Repositories.Oracle.Tests;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        Db = new TestDbContext(
            new OracleConnection(
                "DATA SOURCE=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));User Id=system;Password=Password12!"),
            SqlProvider.Oracle);
        ClearDb();
        InitDb();
    }

    public TestDbContext Db { get; }

    public void Dispose()
    {
        ClearDb();
        Db.Dispose();
    }

    private void InitDb()
    {
        SetupSchema();
        Db.Connection.Execute(
            @"CREATE TABLE USERS (
                  ID NUMBER GENERATED ALWAYS AS IDENTITY(START WITH 1 INCREMENT BY 1),
                  NAME VARCHAR(256) NOT NULL,
                  ADDRESSID NUMBER NOT NULL,
                  PHONEID NUMBER NOT NULL,
                  OFFICEPHONEID NUMBER NOT NULL,
                  DELETED NUMBER,
                  UPDATEDAT DATE,
                  PRIMARY KEY (ID))");

        Db.Connection.Execute(
            @"CREATE TABLE CARS (
                  ID NUMBER GENERATED ALWAYS AS IDENTITY(START WITH 1 INCREMENT BY 1),
                  NAME VARCHAR(256) NOT NULL,
                  USERID NUMBER NOT NULL,
                  STATUS NUMBER NOT NULL,
                  DATA RAW(16) NULL,
                  PRIMARY KEY (ID))");

        Db.Connection.Execute(
            @"CREATE TABLE ADDRESSES (
                  ID NUMBER GENERATED ALWAYS AS IDENTITY(START WITH 1 INCREMENT BY 1),
                  STREET VARCHAR(256) NOT NULL,
                  CITYID VARCHAR(256) NOT NULL,
                  PRIMARY KEY (ID))");

        Db.Connection.Execute(
            @"CREATE TABLE CITIES (
                  IDENTIFIER RAW(16) NOT NULL,
                  NAME VARCHAR(256) NOT NULL)");

        Db.Connection.Execute(
            @"CREATE TABLE DAB.PHONES (
                  ID NUMBER GENERATED ALWAYS AS IDENTITY(START WITH 1 INCREMENT BY 1),
                  PNUMBER VARCHAR(256) NOT NULL,
                  ISACTIVE NUMBER(1) NOT NULL,
                  CODE VARCHAR(256) NOT NULL,
                  PRIMARY KEY  (ID))");

        Db.Connection.Execute(
            @"CREATE TABLE REPORTS (
                  ID NUMBER NOT NULL,
                  ANOTHERID NUMBER NOT NULL,
                  USERID NUMBER NOT NULL,
                  PRIMARY KEY (ID, ANOTHERID))");

        InitData.Execute(Db);
    }

    private void ClearDb()
    {
        void DropTable(string name)
        {
            Db.Connection.Execute($@"BEGIN
                                           EXECUTE IMMEDIATE 'DROP TABLE ' || '{name}';
                                         EXCEPTION
                                            WHEN OTHERS THEN
                                               IF SQLCODE != -942 THEN
                                                  RAISE;
                                               END IF;
                                         END;");
        }

        DropTable("USERS");
        DropTable("CARS");
        DropTable("ADDRESSES");
        DropTable("CITIES");
        DropTable("REPORTS");
        DropTable("PHONES");
    }

    private void SetupSchema()
    {
        Db.Connection.Execute($@"BEGIN
                                        EXECUTE IMMEDIATE 'DROP USER DAB CASCADE';
                                     EXCEPTION
                                         WHEN OTHERS THEN
                                             IF SQLCODE != -1918 THEN
                                                 RAISE;
                                             END IF;
                                     END;");
        Db.Connection.Execute($"CREATE USER DAB IDENTIFIED BY pwd");
        Db.Connection.Execute($"ALTER USER DAB QUOTA UNLIMITED ON USERS");
    }
}
