# MakesCents

Repo for the Makes Cents Capstone Project

## REST API Planning Documentation

[REST API Documentation](RESTAPIDocumentation.md)

# Notes

- Make sure deletes and updates undo the original changes (deleting a transactions means the money is no longer taken out of the account or envelopes)
- Make sure transactions go into the correct budget based on month and year
- Update the gray area when the user is selecting envelopes so that clicking the area outside the box closes it
- Update Transfer Transaction DAO so, if the transfer is updated, the money transfers are correct (possibly undo both money updates are redo them?)
- Update backend so added transactions go into the correct month rather than the current month
- Change transaction delete logic to change to recently deleted instead of hard delete
- Add re-ordering of envelope categories and envelopes
- Add Favorites category
