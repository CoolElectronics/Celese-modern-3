// do i really need to oop everything? seems like a bad idea for this case
public class ExitDefinition
{
    public ExitDefinition(HierarchyObjectUIDefinition source, int index)
    {
        source.inputFields.Add(source.newInputField(viewport, "name", nameDefault, (val) =>
        {

        });
    }
}