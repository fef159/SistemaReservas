using System.Data;
using System.Text;

namespace SistemaReservas.Data;

public static class TableSearch
{
    // Escape each character once: quotes and LIKE wildcards must be literal search text.
    public static string EscapeLiteral(string value)
    {
        var result = new StringBuilder();
        foreach (char c in value)
            result.Append(c switch { '\'' => "''", '[' => "[[]", ']' => "[]]", '%' => "[%]", '*' => "[*]", _ => c.ToString() });
        return result.ToString();
    }

    public static void Apply(DataTable table, string query, params string[] columns)
    {
        string text = EscapeLiteral(query.Trim());
        table.DefaultView.RowFilter = text.Length == 0 ? "" :
            string.Join(" OR ", columns.Select(column => $"[{column}] LIKE '%{text}%'"));
    }
}
