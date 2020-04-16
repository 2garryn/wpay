
CREATE TABLE currency (
    Code VARCHAR(3) PRIMARY KEY
);

INSERT INTO currency (Code) VALUES ('MXN');


CREATE TABLE account (
    Id UUID PRIMARY KEY,
    Balance DECIMAL NOT NULL CHECK (Balance >= 0),
    Currency VARCHAR(3) NOT NULL REFERENCES currency(Code),
    Locked BOOLEAN NOT NULL DEFAULT FALSE 
);


CREATE TYPE TRANSACTION_STATUS AS ENUM ('processing', 'completed', 'cancelled');

CREATE TABLE transaction (
    Id UUID PRIMARY KEY,
    AccountId UUID NOT NULL REFERENCES account(Id),
    Label TEXT NOT NULL,
    Status TRANSACTION_STATUS NOT NULL,
    Amount decimal NOT NULL CHECK (Amount <> 0),
    Currency VARCHAR(3) NOT NULL REFERENCES currency(Code),
    Description JSONB NOT NULL,
    Metadata JSONB NOT NULL,
    CreatedOn TIMESTAMP NOT NULL,
    UpdatedOn TIMESTAMP NOT NULL
);

CREATE TABLE outbox_replication (
    Uniq serial PRIMARY KEY,
    Id UUID NOT NULL,
    EventType TEXT NOT NULL,
    Event JSONB NOT NULL
);