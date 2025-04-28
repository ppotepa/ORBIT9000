using HtmlAgilityPack;
using Terminal.Gui;

public class Translator
{
    private readonly Dictionary<string, Func<HtmlNode, View>> _tagHandlers;

    public Translator()
    {
        _tagHandlers = new Dictionary<string, Func<HtmlNode, View>>(StringComparer.OrdinalIgnoreCase)
        {
            { "nav", CreateNavbar },
            { "section", CreateSection },
            { "h1", node => CreateLabel("# " + node.InnerText.Trim()) },
            { "h2", node => CreateLabel("## " + node.InnerText.Trim()) },
            { "h3", node => CreateLabel("### " + node.InnerText.Trim()) },
            { "p", node => CreateLabel(node.InnerText.Trim()) },
            { "ul", CreateListView },
            { "li", node => CreateLabel("- " + node.InnerText.Trim()) }
        };
    }

    public View Translate(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var root = new View()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        MenuBar navbar = null;

        foreach (var node in doc.DocumentNode.SelectNodes("//body/*") ?? throw new ArgumentException("Tag body not found"))
        {
            var childView = TranslateNode(node);
            if (childView is MenuBar menuBar)
            {
                navbar = menuBar;
            }
            else if (childView != null)
            {
                root.Add(childView);
            }
        }

        if (navbar != null)
        {
            root.Add(navbar);
        }

        return root;
    }

    private Label CreateLabel(string text)
    {
        return new Label(text)
        {
            Width = Dim.Fill(),
            Height = 1
        };
    }

    private ListView CreateListView(HtmlNode node)
    {
        var items = new List<string>();

        foreach (var li in node.SelectNodes(".//li"))
        {
            items.Add(li.InnerText.Trim());
        }

        return new ListView(items)
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
    }

    private MenuBar CreateNavbar(HtmlNode node)
    {
        var menuItems = new List<MenuBarItem>();

        foreach (var li in node.SelectNodes(".//li"))
        {
            var link = li.SelectSingleNode(".//a");
            if (link != null)
            {
                menuItems.Add(new MenuBarItem(link.InnerText.Trim(), new MenuItem[] { }));
            }
        }

        return new MenuBar(menuItems.ToArray());
    }

    private View CreateSection(HtmlNode node)
    {
        var section = new View()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var header = node.SelectSingleNode(".//h1 | .//h2 | .//h3");
        if (header != null)
        {
            section.Add(CreateLabel(header.InnerText.Trim()));
        }

        var paragraph = node.SelectSingleNode(".//p");
        if (paragraph != null)
        {
            section.Add(CreateLabel(paragraph.InnerText.Trim()));
        }

        return section;
    }

    private View TranslateNode(HtmlNode node)
    {
        if (_tagHandlers.TryGetValue(node.Name, out var handler))
        {
            var currentView = handler(node);

            foreach (var childNode in node.ChildNodes)
            {
                var childView = TranslateNode(childNode);
                if (childView != null)
                {
                    currentView.Add(childView);
                }
            }

            return currentView;
        }

        // Default behavior for unsupported tags
        var defaultView = new View()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        foreach (var childNode in node.ChildNodes)
        {
            var childView = TranslateNode(childNode);
            if (childView != null)
            {
                defaultView.Add(childView);
            }
        }

        return defaultView;
    }
}
