## 3-The-Basics-of-Querying-the-Dom
    * find(), children()  (find: all descendents, childrend: direct child)
    * next(), prev()
    * parent(), parents(), closest() (parent: direct parent, parents: find all parents, closest: find first matched parent)
    * siblins()

## 4.Events-101: change stylesheet when button click
    * siblings(), attr(), removeAttr(), end()

## 5.Events-201
    * toggle(), hide(), show(), fadeIn(), slideDown();
    * nth-last-child(an+b): how select last <dt> in <dl>
    <dl>
        <dt></dt>
        <dd></dd>
        <dt></dt>
        <dd></dd>
    </dl>
        * dt:last-child {} will not find any match, because no <dt> at the very last of dl
        * dt:nth-last-child(2) {} // this will select second last <dt>
    * filter(): reduce the set in current selector
        $( "li" ).filter( ":even" ), note: :even and :odd use 0-based indexing
    * code improve
    ````javascript
    // 1.
    $(this).next().siblings('dd').hide();
    $(this).next().show(); // display <dd>
    =======================================>
    $(this)
            .next()
            .show(); // display <dd>
            .siblings('dd')
            .hide();

    // 2. 
    $('dt').on('mouseenter', function() {});
    dont listen all dt elements if has 1000 dt elements, using this:
    $('dl').on('mouseover', 'dt', function() {} )
