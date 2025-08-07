#!/bin/bash

echo "Aguardando PostgreSQL estar pronto..."
sleep 10

echo "Executando migrations..."
dotnet ef database update --project src/Infrastructure

echo "Migrations executadas com sucesso!"
