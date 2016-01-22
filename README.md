### Version history

*	[version 2.7](http://mvlasenko.blogspot.com/2014/04/sdl-tridion-visual-studio-razor.html). Compatible with **Visual Studio 2010-2013** and **Tridion 2013 SP1**. Doesn't include debugging.
*	[version 3.1](http://mvlasenko.blogspot.com/2015/12/sdl-tridion-visual-studio-razor.html). Compatible with **Visual Studio 2015** and **Tridion 2013 SP1**. Includes debugging.
*	[version 4.0](http://mvlasenko.blogspot.com/2016/01/sdl-tridion-visual-studio-razor.html). Compatible with **Visual Studio 2015** and **SDL Web 8**. This version is located in branch **web8**.

### Main functionality

Possibility to work with **Tridion** content in **Microsoft Visual Studio**:

*   Edit razor code for _**Razor Mediator**_ TBBs
*   Synchronize _*.cshtml_ files with Tridion TBBs
*   Debug _*.cshtml_ files
*   Create Tridion_ **Component Templates**_ and_ **Page Templates**_ from Visual Studio automatically
*   Synchronize Visual Studio binaries (*.css, *.js, *.jpg, *.png, etc.) with Tridion multimedia components

### Prerequisites

*   **Microsoft Visual Studio 2015** installed on a local machine
*   **Tridion 2013 SP1** or **SDL Web 8** with **Razor Mediator 1.3.X** installed on the local or remote machine

### Installation

1\. Run _**TridionVSRazorExtension.vsix**_

![](http://4.bp.blogspot.com/-jUCpa9XqvO4/VkN03xEnisI/AAAAAAAAHB4/t6ZoArZfX2o/s1600/rmvs_01.PNG)

2\. Create a new project based on _**RazorMediatorLayouts**_ template

![](http://1.bp.blogspot.com/--Z4vffdjGRQ/VoKbnc3CSMI/AAAAAAAAHGw/tFYbpJW3APc/s1600/new_proj.PNG)

Your new project looks like next

![](http://4.bp.blogspot.com/-KSPBPWyOEG4/VoKbnc7au3I/AAAAAAAAHG8/d-l4aMbR_qI/s1600/project.PNG)

In Tools menu you have the option _**Tridion Razor Extension**_

![](http://4.bp.blogspot.com/--am56Uds2qw/VkN04bPakFI/AAAAAAAAHC0/puj9UzfDB60/s1600/rmvs_04.PNG)

3\. You can use the dialog window for adjusting _**VS / TCM**_ mapping and running _**VS / TCM**_ synchronization

![](http://3.bp.blogspot.com/-LAXrswI1Tiw/VkN04eXNmqI/AAAAAAAAHDY/slXv49ySc3A/s1600/rmvs_05.PNG)

4\. You can use context menu on an item or a folder in _**Solution Explorer**_ to run _**VS / TCM**_ synchronization

![](http://3.bp.blogspot.com/-k5xKxjUT1to/VoKbnpuT1OI/AAAAAAAAHHE/jEMN4J0Y3YI/s1600/right_click.PNG)

### Mapping

_* All mapping info is stored in TridionRazorMapping.xml file in the project root._

#### TCM mapping

Select folders on TCM side.

![](http://2.bp.blogspot.com/-rI8o83qAFzQ/VkN04lBdR6I/AAAAAAAAHDI/rRr2lX16tgo/s1600/rmvs_06.PNG)

Every _**Tridion** _folder must have one of the following roles:

*   **PageLayoutContainer**. Folder that contains Razor Mediator TBBs for page layouts.
*   **ComponentLayoutContainer**. Folder that contains Razor Mediator TBBs for components.
*   **PageTemplateContainer**. Folder that contains Page Templates.
*   **ComponentTemplateContainer**. Folder that contains Component Templates.
*   **MultimediaComponentContainer**. Folder that contains Multimedia Components.
*   **Other**. No synchronization actions defined for this folder.

By default synchronization works only in VS -> TCM direction. To enable two-way synchronization, please use _**Two-way**_ checkbox.

_* TCM -> VS synchronization is more time-consuming, since TCM folders are scanned for new items._

#### Visual Studio mapping

Select folders from Visual Studio MVC project.

![](http://3.bp.blogspot.com/-BHl0m51eeeQ/VkN04ixHQKI/AAAAAAAAHDM/iNmPbZi-Wac/s1600/rmvs_07.PNG)

Use double-clicks to edit existing folder settings or the “_**Add**_” button to add a new folder.

![](http://3.bp.blogspot.com/-4gqC1XW129M/VkN04nbhtZI/AAAAAAAAHDc/KqnTQJEQ8jQ/s1600/rmvs_07.1.PNG)

In this dialog you can change the folder and file settings.
 1\. **Role**. Select project folder role:

*   **PageLayout**. Folder contains *.cshtml files for page templates.
*   **ComponentLayout**. Folder contains *.cshtml files for component templates.
*   **Binary**. Folder contains binaries (*.jpg, *.png, *.js, *.css, etc)
*   **Other**. Folder action is not defined.

If the files of different purposes are mixed in the same folder on the file system you can map this folder twice and select only the files of the specific type. All the checked files inherit role from the parent folder.

2\. **Template**. Whether to create the page or component template after *.cshtml file was synchronized to _Razor TBB_. (Only _PageLayout_ and _ComponentLayout_ roles)

3\. **Tridion Folder**. If you have more than one Tridion folder you can select which folder to synchronize.

4\. **Template Format**. Piece of XML that is used to create _**Compound Component Templates**_ from _Razor TBB_. All the checked files inherit role from parent folder.

5\. **Schema Names**. List of schema names to link to _Component Compound Template_ on creating.

### Synchronization

You can run synchronization in three ways:

#### 1\. Dialog window – “Run Synchronization”.

Runs synchronization for all the mapped Tridion and project folders.

*	**Item exists only in project**. The user will be prompted to enter the name of a new Tridion item and select the target publication

![](http://3.bp.blogspot.com/-ISxXYe6ff_A/VkN05Yqk1PI/AAAAAAAAHDQ/Zzsf40RViKs/s1600/rmvs_12.PNG)

*	**Item exists only in Tridion** (_**two-way**_ must be enabled for Tridion folder(s))

User will be prompted to enter the name of a new project item and select the project folder to place it.

![](http://1.bp.blogspot.com/-wBjaGsHk0Qw/VkN04yZEYlI/AAAAAAAAHCs/GzUdEwg-7UU/s1600/rmvs_08.PNG)
*	**Item exists in both project and Tridion**. The application compares dates and suggests the user which version to use.

![](http://4.bp.blogspot.com/-4jVb0KNhLGk/VkN05OlloII/AAAAAAAAHDE/Svie1pGYb54/s1600/rmvs_10.PNG)

_* It is important to select the proper time zone to have the proper time comparison_

The user can make a decision which version to use

_* You won’t lose your previous changes. If you synchronize a Tridion item from the project then the new Tridion item version will be created, and you will be able to roll it back in TCM. If your project item is overwritten by a Tridion item then you can use your version control system to roll it back_

#### 2\. Context menu on file(s) or folder

This kind of synchronization will synchronize only selected items.

![](http://3.bp.blogspot.com/-JBi85N4vGDs/VoKdgM6zkTI/AAAAAAAAHHQ/K_01FjGrD7A/s1600/context.PNG)

### Creating of component templates

If **_*.cshtml_** file was created in Visual Studio, and you want to create component template automatically, please use the next mapping settings:

![](http://1.bp.blogspot.com/-VUiBFgPHHME/VkN05Niy_cI/AAAAAAAAHC4/rR20lh32QyI/s1600/rmvs_11.PNG)

After synchronization is run, you will have template created automatically

![](http://2.bp.blogspot.com/-JJJ6JPkoY4M/VkN05QsLQSI/AAAAAAAAHC8/SqqDent9-qo/s1600/rmvs_13.PNG)

### Debugging

Before start debugging you need to set configuration for _**TcmDebugger**_. Please read _**[Debugging Razor Mediator templates](http://mvlasenko.blogspot.com/2015/12/debugging-razor-mediator-templates.html)**_

You can start debugging synchronization in two ways:

*   From item context menu

![](http://3.bp.blogspot.com/-JBi85N4vGDs/VoKdgM6zkTI/AAAAAAAAHHU/GbqmzN3kQck/s1600/context.PNG)

*   Press _**Run** _from Visual Studio toolbar. Then the latest **_.cshtml_** will be run

If razor TBB was not debugged before you will be requested to set test _**component / page**_ and test _**template**_ that includes current layout

![](http://1.bp.blogspot.com/-3N1Q3XI_kXI/VoKq9XssSaI/AAAAAAAAHIY/wYhOyYgaDvw/s1600/debug.PNG)

You can use breakpoints

![](http://2.bp.blogspot.com/-zDbgJ2dLhmc/VoPC-BctlHI/AAAAAAAAHIs/F6JvB6rHksg/s1600/debug_screen.PNG)

### Work with razor helpers

Application analyses _**Tridion.ContentManager.config**_ and finds helpers on the file systemand makes a copy file in Visual Studio project:

![](http://4.bp.blogspot.com/-7hzvCNdmElo/VoKke63TA5I/AAAAAAAAHH0/NFXf1Aix4L8/s1600/project2.PNG)

To use this helper methods in other **_.cshtml_** files we need a customized version of _**Razor Generator**_

![](http://3.bp.blogspot.com/-UmnK_paWOtg/VoKbnRLKt7I/AAAAAAAAHHI/fFmhokNZXqU/s1600/generator.PNG)

Make sure that helper file is marked with custom tool _**RazorGenerator**_

![](http://1.bp.blogspot.com/-cX4LwN6kWRg/VoKkei9QnFI/AAAAAAAAHH8/0d3E08P6Y28/s1600/custom_tool.PNG)

Also make sure that razor file has the next directive on the top. This is a marker that proper code generation is used

![](http://2.bp.blogspot.com/-wATidgpFuJM/VoKkekoFomI/AAAAAAAAHIE/MhEMnGtuD9o/s1600/directive.PNG)

Then you will see file _**RazorHelpers.generated.cs**_ with the next generated C# code

![](http://1.bp.blogspot.com/-wZ5mCf0s8Mg/VoKkeuvDHwI/AAAAAAAAHH4/SWKh1atXEDU/s1600/generated.PNG)

Since this code is included into base _**WrappedTridionRazorTemplate**_ class it can be used in any razor file

![](http://2.bp.blogspot.com/-VFK_J28-JaI/VoKoF645psI/AAAAAAAAHIM/-y1_QAYrxbw/s1600/helper_use.PNG)

_* If helpers are located in standalone TBB instead of file system and referenced via import directive_

Just set custom tool _**RazorGenerator**_ for helper TBB

## Downloads

[https://drive.google.com/folderview?id=0B6hZ-cambftrV25ZLU5QTnJWSmM&usp=sharing#list](https://drive.google.com/folderview?id=0B6hZ-cambftrV25ZLU5QTnJWSmM&usp=sharing#list)