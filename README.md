# SqlVirtualEntityDataProvider
A Virtual Entity Data Provider using a direct SQL Connection to Azure SQL / SQL On-Prem. With contributions from <a href='https://github.com/MarkMpn/' target='_blank'>Mark Carrington</a> and <a href='https://github.com/rappen' target='_blank'>Jonas Rapp</a>. Relies on <a href='https://github.com/MarkMpn/Sql4Cds' target='_blank'>Sql4CDS</a> from Mark Carrington.

The project includes a manaaged / unmanaged solution that you can import and configure for a quick start to creating SQL based Virtual Entities as well as the source code to customize to your specific requirements.

<!-- wp:paragraph -->
Features:

Display SQL Tables as Virtual Entities using only a connection string to your SQL instance
Automatically convert FetchXml queries to SQL via Sql4Cds.
Automatically convert integer key values to guids without the need for a guid column in the Azure SQL table
Display Virtual Entities as lookups on OOB or custom entities or other virtual entities.

You can get started using the Sample App using the instructions below. More to follow with respect to configuring the Provider for personal / corporate use outside of the provided sample App.
<!-- wp:list {"ordered":true} -->
<ol><li>Required: A Dynamics 365 or Power Apps Subscription</li><li>Required: An Azure Subscription with Rights to Create Resources</li><li>Optional: SQL Server Management Studio (to test your connection)</li></ol>
<!-- /wp:list -->

<!-- wp:heading {"level":4} -->
<h4>Setting Up Your Sample SQL Azure Instance</h4>
<!-- /wp:heading -->

<!-- wp:image {"id":688,"sizeSlug":"large"} -->
<figure class="wp-block-image size-large"><img src="https://mikefactorial.com/wp-content/uploads/2020/08/image-4-1024x464.png" alt="" class="wp-image-688"/><figcaption>Configure a new Azure SQL Server and Database</figcaption></figure>
<!-- /wp:image -->

<!-- wp:list {"ordered":true} -->
<ol><li>Navigate to portal.azure.com and signin</li><li>Click Create a Resource + Databases</li><li>Click SQL Database or Managed Instance</li><li>Select your Subcription and Resource Group</li><li>Select Create New if you don't already have a SQL Server Instance<ol><li>Enter Server Name</li><li>Enter Server Admin Login</li><li>Enter and Confirm the Password for your Admin Account</li><li>Select Okay</li></ol></li><li>Now Click on Networking<ol><li>Set Connectivity Method to Public Endpoint</li><li>Select Add Current Client IP Address and Allow Azure Services to Access this server</li></ol></li><li>Click Additional Settings<ol><li>Next to Use existing data select Sample</li></ol></li><li>Select the remaining defaults to create your Azure SQL Server and Database</li><li>Once the server and database are provisioned go to the resource and select Connection Strings from Menu</li><li>Copy the ADO.NET Connection String to use in the next step</li></ol>
<!-- /wp:list -->

<!-- wp:image {"id":690,"sizeSlug":"large"} -->
<figure class="wp-block-image size-large"><img src="https://mikefactorial.com/wp-content/uploads/2020/08/image-5-1024x298.png" alt="" class="wp-image-690"/></figure>
<!-- /wp:image -->

<!-- wp:heading {"level":4} -->
<h4>Installing and Configuring the Solution</h4>
<!-- /wp:heading -->

<!-- wp:list {"ordered":true} -->
<ol><li>Download either the <a rel="noreferrer noopener" href="https://github.com/mikefactorial/SqlVirtualEntityDataProvider/tree/master/SqlVirtualEntityDataProvider/Solutions" target="_blank">Unmanaged or Managed (recommended) SQL Virtual Entity Sample Solution</a>. (NOTE: I plan to grow this over time and add more features, but it's a good starting point and I welcome feedback and contributions. A big thanks to <a rel="noreferrer noopener" href="https://github.com/rappen" target="_blank">@rappen</a> for allowing me to use some of his code to translate FetchXML to SQL that he has included as open source in <a rel="noreferrer noopener" href="https://fetchxmlbuilder.com/" target="_blank">FetchXML Builder</a> with some slight modifications.)<ul><li>NOTE: There are 4 solutions available both the managed / unmanaged solution for the Sample App, which contains the preconfigured entities for the sample SQL database and the Provider only App that contains only the provider so you can roll your own Virtual Entities.</li></ul></li><li>Import the solution by going to make.powerapps.com and logging into your tenant.</li><li>Select the environment you want to import the solution into.</li><li>Go to Solutions and select Import and select the solution you downloaded.<ol><li>After the Solution has been imported you should see a new Model Driven App under Apps called "Azure SQL Product Catalog Sample"</li><li>NOTE: This app is based on the sample data that is installed with the Azure SQL database when you choose to install sample data. In the next article I'll walk through how I created this app so you can create your own using the same Data Provider.</li></ol></li></ol>
<!-- /wp:list -->

<!-- wp:image {"id":693,"sizeSlug":"large"} -->
<figure class="wp-block-image size-large"><img src="https://mikefactorial.com/wp-content/uploads/2020/08/image-7-1024x214.png" alt="" class="wp-image-693"/></figure>
<!-- /wp:image -->

<!-- wp:list {"ordered":true} -->
<ol><li> The final step to configuring the app to display Products and Product Categories from Azure SQL is to configure the Provider using your specific SQL Connection String.<ul><li>Open the Azure SQL Product Catalog Sample App (NOTE: You'll see an error when you launch the app because you haven't configured the connection)</li></ul></li></ol>
<!-- /wp:list -->

<!-- wp:image {"id":695,"sizeSlug":"large"} -->
<figure class="wp-block-image size-large"><img src="https://mikefactorial.com/wp-content/uploads/2020/08/image-8-1024x453.png" alt="" class="wp-image-695"/></figure>
<!-- /wp:image -->

<!-- wp:list -->
<ul><li>Open Advanced Find<ul><li>Search for 'SQL Providers'</li><li><strong>Open the record that is returned and paste the SQL Connection String from Azure SQL and Save the Record</strong><ul><li>IMPORTANT: You will need to replace the {your_password} in the connection string with your password.</li></ul></li></ul></li></ul>
<!-- /wp:list -->

<!-- wp:image {"id":697,"sizeSlug":"large"} -->
<figure class="wp-block-image size-large"><img src="https://mikefactorial.com/wp-content/uploads/2020/08/image-9-1024x351.png" alt="" class="wp-image-697"/></figure>
<!-- /wp:image -->

<!-- wp:list -->
<ul><li>Click on Products or Product Categories from the Left Menu to see the Products and Categories stored in Azure SQL.<ul><li>NOTE: If you get an error at this point it could be related to your Azure SQL Firewall and as such you may need to allow specific IPs through.</li></ul></li></ul>
<!-- /wp:list -->
