# Queries

Queries are what the system uses to read data.

Reading data can be done in 2 ways. 

- For queries that are purely for the frontend, a read-only data store can be 
  prepared with the relevant data. This read-only data store is being updated
  by new events triggering all the time.
- For queries that are being used in commands, a read-only model is insufficient
  as this is not the "single point of truth". Therefor these queries should
  query the event store and replay the events to get to the latest version of 
  the data, regardless on what state the read-only model is.

