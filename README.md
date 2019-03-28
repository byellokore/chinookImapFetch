# chinookImapFetch
Task made for an Interview test from Chinook Canada

#CASE:
You need to scan thru thousands of emails located on email server (IMAP). The idea is to find emails that includes the following codes somewhere in the emails body:
* Bill of Lading code ( Format examples can be found at:https://en.wikipedia.org/wiki/Bill_of_lading#How_to_recognize_a_valid_international_bill_of_lading )
* Container code ( Format: always 4 chars plus 7 digits, e.g. MSVU1234567 )
* From the emails subject line only: 4 digit Sales Order code between 6000 and 7999 (For example subject line could be "RE: BOOKING REQUEST PO 7251 12 JAN 2018")
 
- You need to store those values as an objects for later handling only from the emails where all 3 codes exists. Not every email has those.
- The 4 digit Sales Order code needs to be an unique, only one entry can exist, but you need to update its Container and/or Bill of Lading numbers if the email has more recent data for the Sales Order number.
- If the email matching the previous conditions has any number of attachments, store filename(s) in the related object as well.
 
How would you solve the problem? What tools or techniques would you use?
Definitely no need to write full working solution; simple explanations of the design and different steps are good but code samples of some of the methods and classes are really welcome (Pref. C# [.NET]).
 
BONUS:
What has been your hardest or most interesting bug in the code you had to solve?
Explain how you managed to solve it. What made it difficult?

