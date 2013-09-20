Chain
=====

A very small library that should help you clean your code. 

Usage
-----

Fairly simple, you chain calls with fallbacks. It works mostly with function pointers (fancy term, in C# you just need to use the function name). Simple example : 

```
public class InvoiceGenerator
{
	public Invoice CreateInvoice(int customerId) {
		return Chain.Do(FindCustomer(customerId))
				.Then(LoadCustumerLatestPurchases).OrElse(new List<Purchase>())
				.Then(GenerateInvoice)
				.Then(AddTaxes);
	}
	
	private Customer FindCustomer(in customerId) { // ... may return null }
	
	private List<Purchases> LoadCustumerLatestPurchases(Customer customer) { // ... no need to check for null here }
	
	private Invoice GenerateInvoice(List<Purchases> purchases) { // ... }
	
	private Invoice AddTaxes(Invoice invoice) { // ... }
}
```

How easy is that to read? Let's break it down though : 

1. `FindCustomer(int customerId)` : `Do` takes either a function pointer or an object. In this case it returns an object, which might be null.
2. `LoadCustomerLatestPurchases`: This method will get the previous result passed as first argument. In case the previous call returned null, this step will not even be invoked, so no need to check for nulls.
3. `OrElse` : This takes either a fallback object like here, or a function pointer. In this case, if either `FindCustomer` or `LoadCustumerLatestPurchases` returns null, this empty list will be used to go on. Again, no need for manual null checking.
4. `GenerateInvoice` : This will be passed a list of purchases as an argument, since it's the result from the previous chain. Since we used an `OrElse`, no need to worry about nulls, an empty list will be given. 
5. `AddTaxes` : In case `GenerateInvoice` returns null, this won't even be invoked and the total chain result will be null. However, if GenerateInvoice returns something (which is most likely - even in case of an empty purchase list), this will be given the invoice and return it.
6. Implicit conversion will take place to return an instance of Invoice and not an instance of the underlying construct. 

VB.NET users
------------

VB.NET loves to add random useless keywords to the language, so you will need to use something like `Then(AddressOf MyFunctionName)` instead.

F# users
--------

What are you doing here? F# has this construct implemented in the language itself!

Disclaimer
----------

This is still in early development. If you have feature ideas, you are welcome to submit a pull request.

Licence
-------
Published under MIT licence, see the LICENCE.txt file

				