-- The "slow query of the week".
-- 
-- Business question: for a given event, show me the available seats
-- ordered by price ascending (cheapest first), with section/row/number
-- info so a customer could pick one.
--
-- This is intentionally NOT optimized. No indexes have been added
-- beyond what EF Core created automatically (primary keys + FK columns).
-- We'll come back to this on Thursday with EXPLAIN ANALYZE.
-- Pick a busy event so the result set is non-trivial. Event 1 has
-- ~2,300 available seats; tweak as needed.
EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)
SELECT
    id,
    section,
    row,
    number,
    price
FROM
    seats
WHERE
    event_id = 1
    AND status = 'available'
ORDER BY
    price ASC
LIMIT
    50;

-- Same query, parameterized for trying different events.
-- Uncomment and edit the event_id to compare plans across events:
--
-- EXPLAIN (ANALYZE, BUFFERS) 
-- SELECT id, section, row, number, price
-- FROM seats
-- WHERE event_id = 2 AND status = 'available'
-- ORDER BY price ASC
-- LIMIT 50;