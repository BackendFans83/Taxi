#!/bin/bash
set -e

# Функция для создания базы и пользователя
create_db_and_user() {
    local dbname=$1
    local username=$2
    local password=$3

    echo "Creating database '$dbname' with user '$username'..."

    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
        CREATE USER $username WITH PASSWORD '$password';
        CREATE DATABASE $dbname;
        GRANT ALL PRIVILEGES ON DATABASE $dbname TO $username;
EOSQL
}

create_db_and_user "user_service" "user_service__user" "user_service__pass123"
create_db_and_user "ride_service" "ride_service__user" "ride_service__pass123"

echo "Databases initialized successfully!"