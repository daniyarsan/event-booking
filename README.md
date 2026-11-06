# EventBooking

## Local PostgreSQL

Start the database with Podman Compose:

```sh
podman compose up -d
podman compose ps
```

The first start creates a `reservation` database and user with password
`reservation`. PostgreSQL is available only on the local host at port `5432`.
These credentials are for local development. Set `POSTGRES_DB`, `POSTGRES_USER`,
`POSTGRES_PASSWORD`, or `POSTGRES_PORT` in your environment before starting to
override the defaults.

Connect from the host using `Host=localhost;Port=5432;Database=reservation;Username=reservation;Password=reservation`,
or open a SQL session inside the container:

```sh
podman compose exec postgres psql -U reservation -d reservation
```

The named volume preserves database data when containers are stopped or recreated.
The initialization variables only apply to a new, empty volume; changing them
later does not change the existing database or credentials.

The API does not yet have database access or migrations, so this starts an empty
database for the reservation domain. Domain tables must be added through a future
persistence implementation and migrations.
