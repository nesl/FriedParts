FriedParts
==========

Overview
--------

Computer Aided Design (CAD) tools have all but eliminated manual drafting and for good reason. CAD offers more accurate drawing tools, grey coding of drafting symbols, rapid duplication of finished designs, easier integration with manufacturing partners, and automated design rule verification. For the purposes of this work we focus on electronics design. CAD tools in this space manage part data in a logical schematic view (a part symbol) and a physical PCB view (a part footprint). Yet, a part has a third view, which CAD tools ignore -- it's supply data (Manufacturer part number, variant, distributor, etc). 

To manage this manufacturing view a broad class of tools known as Product Lifecycle Management (PLM) have evolved to include most of the functions previously classified as Engineering Resource Planning (ERP). AMR Research analyst Michael Burkett describes it this way, "PLM is [no longer] a single application but a process that crosses multiple business processes and technologies, i.e. marketing and supply chain." 

Despite these advances, a substantial chiasm still exists between the manufacturing and engineering views. More specifically, part data known to the supply chain (managed through PLM tools) and performance and specification data known to the engineering world (managed through CAD tools) must be manually integrated and managed by the design team. 

This leads to a substantial amount of redundant data entry into both tool chains with any error resulting in an inconsistency between design intent and fabrication. In the case of electronic small parts, this can result in subtle (even unnoticeable) part substitutions which may yield dramatic product performance alterations. 

In this work we introduce an entirely new approach to bridging the tool-chain divide -- a web-based architecture we call FriedParts. FriedParts exploits the recently available database-driven parametric part interfaces of CAD tools (like Cadence's Component Information System or Altium's Database Library Components) and web 2.0 automation to crawl data information providers like Octopart, Inc. and Digikey, Inc. and tie this information directly into the CAD tool at design time. It uses heuristics to suggest CAD symbols and footprints. Part search is handled from the website where cloud computing accelerates the search performance. The materials bill output from the CAD tool is then fed back into FriedParts which can automatically find second-source distribution, find alternate manufacturers, optimize purchasing, and perform other PLM functions. The amount of data entry by the designer is brought to almost zero. FriedParts stores the actual CAD data (part libraries) fostering verification and collaboration. FriedParts is open-source and will be made available as a free service.

FriedParts was principally developed by *Jonathan Friedman* of the *Networked and Embedded Systems Lab* (NESL) of the *University of California, Los Angeles* (UCLA). 

Project Manager
---------------
Jonathan Friedman, MSEE PhDc
jf@ee.ucla.edu

Resources
---------
* [The "live" site running this code](http://friedparts.nesl.ucla.edu/FriedParts)
* [The 2011 IPC APEX conference paper on the FriedParts architecture](http://nesl.ee.ucla.edu/document/show/355)
* [The UCLA NES Laboratory project page for FriedParts](http://nesl.ee.ucla.edu/project/show/70)

Significant Initial Contributors
-------------------------------
Significant contributions have also been made by:

* Newton Truong
* Zainul Charbiwala
* The [Dropbox](http://www.dropbox.com) team
* The [Octopart](http://octopart.com) team


Licensing and Terms of Use
==========================
This project and all of its components and assets (images, code, files, libraries, and components) are released under the following license terms and conditions.

Copyright
---------
All assets are copyright (c) 2011, Regents of the University of California, except where indicated (certain included libraries are copyright their respective authors). To the maximum extent possible, all rights are reserved.

License (Simplified BSD)
------------------------
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
* Neither the name of the University of California, Los Angeles nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

Disclaimer
----------
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.