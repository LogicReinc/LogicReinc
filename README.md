# LogicReinc

The LogicReinc Framework is a framework I have been building for many years now and was mainly for personal use.
Now I'm slowly recreating the library, moving code to this one, making it available for everyone. This means that not all functionality has been transfered (and corrected) yet, this is only a small part.
This library has no specific purpose. Its a large toolbox. 

However, this framework consists out of 2 parts. The LogicReinc framework and the LogicReinc.Data. (I will most likely split these at some point)

#Features
While its hard keeping this list up to date, here are a few of the functionalities.. 
 - Thread-Safe collections with similar/same syntax as their normal version
 - Utility objects
 - Various extensions on basic types
 - Javascript Builder
 - Simple XML Parser (Wrapper around .Net's XmlSerializer but inspired by JsonConvert design) 
 - Token System
 - Cryptographics class for quick and easy hashing, encrypting etc
 - Syntax sugar objects like a Switch object that bypasses the constant limitation and a Try object for inline-use
 - WorkerPool which acts as an instanced version of .NETs ThreadPool

# LogicReinc.Data
LogicReinc.Data contains code to connect to many different databases. There are also different ways of using them.
My favorite being the Unified framework, Its an in-memory system that was build to prevent you having to implement multiple databases.
It has an "unified" interface which you use in your objects, while the only difference is settings the "Provider". Which indicates how and what database type it should connect.


Note that these 2 projects contain work-in-progress code. Don't mindlessly use this library.
