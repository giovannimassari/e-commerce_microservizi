#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE DATABASE utenti_db;
    CREATE DATABASE magazzino_db;
    CREATE DATABASE approvvigionamento_db;
    CREATE DATABASE pagamenti_db;
EOSQL