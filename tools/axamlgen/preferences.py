#!/usr/bin/env python3
"""Writes Source/ULDB/Builder/Windows/PreferencesFormView.axaml, the window of upstream's PreferencesForm.

Usage: preferences.py OUT.axaml

Controls are named like upstream's fields (ModelBinder.BindByName binds them); labels that
upstream keeps in designer locals get their text here. Layout: one card per upstream group
box, rows of "label: control", every label column shared per tab (SharedSizeGroup).
"""
import sys

def rows(items, shared="lbl"):
    """items: (label or None, xaml) pairs -> a two-column grid."""
    out = ['<Grid RowSpacing="6" ColumnSpacing="10"><Grid.ColumnDefinitions><ColumnDefinition Width="Auto" SharedSizeGroup="%s"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions><Grid.RowDefinitions>%s</Grid.RowDefinitions>' % (shared, '<RowDefinition Height="Auto"/>' * len(items))]
    for i, (label, x) in enumerate(items):
        if label is None:
            out.append(x.replace('<', '<', 1).replace(' ', ' Grid.Row="%d" Grid.Column="0" Grid.ColumnSpan="2" ' % i, 1) if False else '<ContentPresenter Grid.Row="%d" Grid.ColumnSpan="2">%s</ContentPresenter>' % (i, x))
        else:
            out.append('<TextBlock Grid.Row="%d" Text="%s" Classes="uldb-field" HorizontalAlignment="Right"/>' % (i, label))
            out.append('<ContentPresenter Grid.Row="%d" Grid.Column="1">%s</ContentPresenter>' % (i, x))
    out.append('</Grid>')
    return ''.join(out)

def card(title, *children):
    return '<Border Classes="uldb-group"><StackPanel Spacing="6"><TextBlock Text="%s" Classes="uldb-group-title"/>%s</StackPanel></Border>' % (title, ''.join(children))

def slider(name, label, width=170):
    return '<StackPanel Orientation="Horizontal" Spacing="8"><Slider x:Name="%s" Width="%d" MinHeight="32" VerticalAlignment="Center" IsSnapToTickEnabled="True" TickFrequency="1"/><TextBlock x:Name="%s" VerticalAlignment="Center" MinWidth="70" Opacity="0.85"/></StackPanel>' % (name, width, label)

def chk(name):
    return '<CheckBox x:Name="%s"/>' % name

def combo(name, width=200):
    return '<ComboBox x:Name="%s" Width="%d"/>' % (name, width)

def color(name):
    return '<ContentControl x:Name="%s" Width="250" HorizontalAlignment="Left"/>' % name

def stack(*c, spacing=4):
    return '<StackPanel Spacing="%d">%s</StackPanel>' % (spacing, ''.join(c))

def num(name):
    return '<ContentControl x:Name="%s" Width="92" HorizontalAlignment="Left"/>' % name

def scroll(inner):
    return '<ScrollViewer HorizontalScrollBarVisibility="Disabled"><Grid Grid.IsSharedSizeScope="True" Margin="0,10,10,10">%s</Grid></ScrollViewer>' % inner

def cols(*parts, widths=None):
    n = len(parts)
    widths = widths or ['*'] * n
    out = '<Grid ColumnSpacing="10"><Grid.ColumnDefinitions>%s</Grid.ColumnDefinitions>' % ''.join('<ColumnDefinition Width="%s"/>' % w for w in widths)
    for i, p in enumerate(parts):
        out += '<StackPanel Grid.Column="%d" Spacing="10">%s</StackPanel>' % (i, p)
    return out + '</Grid>'

# ---- Interface
interface = cols(
    stack(
        card('Options', rows([
            ('Default view:', combo('defaultviewmode', 160)),
            ('Auto-scroll speed:', slider('autoscrollspeed', 'autoscrollspeedlabel')),
            ('Zoom factor:', slider('zoomfactor', 'zoomfactorlabel')),
            ('Vertex scale (2D):', slider('vertexScale', 'vertexScaleLabel')),
            ('Max. recent files:', slider('recentFiles', 'labelRecentFiles')),
            (None, stack(chk('scriptontop'), chk('showtexturesizes'), chk('texturesizesbelow'), chk('locatetexturegroup'),
                         chk('blackbrowsers'), chk('cbSynchCameras'), chk('cbStoreEditTab'), chk('checkforupdates'), chk('autolaunchontest'))),
        ])),
        card('ULDB', rows([
            ('Interface scale:', '<StackPanel Orientation="Horizontal" Spacing="8"><ComboBox x:Name="uldb_scale" Width="140"/><TextBlock Text="restart to apply" Opacity="0.7" FontSize="12" VerticalAlignment="Center"/></StackPanel>'),
            ('Texture browser background:', combo('uldb_browserbg', 200)),
            ('Thing rotation per step:', '<StackPanel Orientation="Horizontal" Spacing="8"><ComboBox x:Name="uldb_rotation" Width="100"/><TextBlock Text="degrees (mouse wheel)" Classes="uldb-field"/></StackPanel>'),
            ('Tags when inserting prefabs:', combo('uldb_prefabtags', 240)),
        ], 'lbl0')),
        card('Side Panels', '<StackPanel Orientation="Horizontal" Spacing="10"><TextBlock Text="Position:" Classes="uldb-field"/><ComboBox x:Name="dockersposition" Width="110"/><CheckBox x:Name="collapsedockers"/></StackPanel>'),
    ),
    stack(
        card('Visual Modes', rows([
            ('Field of view:', slider('fieldofview', 'fieldofviewlabel', 150)),
            ('Mouse speed:', slider('mousespeed', 'mousespeedlabel', 150)),
            ('Move speed:', slider('movespeed', 'movespeedlabel', 150)),
            ('Vertex scale (3D):', slider('vertexScale3D', 'vertexScale3DLabel', 150)),
            ('View distance:', slider('viewdistance', 'viewdistancelabel', 150)),
            (None, chk('invertyaxis')),
        ], 'lbl2')),
        card('Toolbar Buttons', '<UniformGrid Columns="2">' + ''.join(chk(n) for n in [
            'toolbar_file', 'toolbar_filter', 'toolbar_script', 'toolbar_viewmodes', 'toolbar_undo', 'toolbar_geometry',
            'toolbar_copy', 'toolbar_testing', 'toolbar_prefabs', 'toolbar_gzdoom']) + '</UniformGrid>'),
        card('Text Labels', '<StackPanel Orientation="Horizontal" Spacing="8"><TextBlock Text="Font:" Classes="uldb-field"/><ComboBox x:Name="textlabelfontname" Width="170"/><TextBlock Text="Size:" Classes="uldb-field"/><ComboBox x:Name="textlabelfontsize" Width="76"/><CheckBox x:Name="textlabelfontbold"/></StackPanel>'),
        card('Screenshots Folder', '<Grid ColumnDefinitions="*,Auto,Auto" ColumnSpacing="4"><TextBox x:Name="screenshotsfolderpath"/><Button x:Name="browsescreenshotsdir" Grid.Column="1" Classes="uldb-tool"/><Button x:Name="resetscreenshotsdir" Grid.Column="2" Classes="uldb-tool"/></Grid>'),
    ), widths=['*', '*'])

# ---- Controls (keys)
keys = '''<Grid ColumnDefinitions="*,12,330" Margin="0,10,0,0">
  <DockPanel>
    <Grid DockPanel.Dock="Top" ColumnDefinitions="Auto,*,Auto" ColumnSpacing="6" Margin="0,0,0,6">
      <TextBlock Text="Filter:" Classes="uldb-field"/><TextBox x:Name="tbFilterActions" Grid.Column="1" Watermark="Action name"/><Button x:Name="bClearActionFilter" Grid.Column="2" Classes="uldb-tool"/>
    </Grid>
    <Border Classes="uldb-grid" BorderThickness="1" CornerRadius="3" ClipToBounds="True">
      <ListBox x:Name="listactions" SelectionMode="Single">
        <ListBox.ItemTemplate><DataTemplate x:DataType="shell:ModelBinder+ListItem">
          <Grid ColumnDefinitions="*,Auto" Margin="4,1"><StackPanel><TextBlock Text="{Binding Text}"/><TextBlock Text="{Binding Group}" FontSize="11" Opacity="0.55"/></StackPanel><TextBlock Grid.Column="1" Text="{Binding SubText}" Opacity="0.8" VerticalAlignment="Center" Margin="12,0,0,0"/></Grid>
        </DataTemplate></ListBox.ItemTemplate>
      </ListBox>
    </Border>
  </DockPanel>
  <Border x:Name="actioncontrolpanel" Grid.Column="2" Classes="uldb-group"><StackPanel Spacing="8">
    <TextBlock x:Name="actioncontrolpanelTitle" Classes="uldb-group-title"/>
    <TextBlock x:Name="actiontitle" FontWeight="SemiBold" TextWrapping="Wrap"/>
    <TextBox x:Name="actiondescription" IsReadOnly="True" AcceptsReturn="True" TextWrapping="Wrap" Height="90" Opacity="0.85"/>
    <TextBlock Text="Press the desired key combination here:" Classes="uldb-field" TextWrapping="Wrap"/>
    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="4"><TextBox x:Name="actionkey" IsReadOnly="True"/><Button x:Name="actioncontrolclear" Grid.Column="1" Classes="uldb-tool"/></Grid>
    <TextBlock Text="Or select a special input control here:" Classes="uldb-field" TextWrapping="Wrap"/>
    <ComboBox x:Name="actioncontrol" HorizontalAlignment="Stretch"/>
    <TextBlock x:Name="disregardshiftlabel" TextWrapping="Wrap" FontSize="12" Opacity="0.8"/>
    <TextBlock x:Name="keyusedlabel" TextWrapping="Wrap" FontSize="12"/>
    <ListBox x:Name="keyusedlist" Height="110"/>
  </StackPanel></Border>
</Grid>'''

# ---- Appearance
appearance = cols(
    card('Colors', stack(*[color(n) for n in ['colorbackcolor', 'colorvertices', 'colorlinedefs', 'colorhighlight', 'colorselection', 'colorindication',
                                              'colorgrid', 'colorgrid64', 'colorMD3', 'colorInfo', 'colorguidelines', 'color3dFloors']], spacing=6)).replace('<Border ', '<Border x:Name="colorsgroup1" ', 1),
    card('Rendering', rows([
        ('Passable lines transparency:', slider('doublesidedalpha', 'doublesidedalphalabel')),
        ('Things transparency (Things mode):', slider('activethingsalpha', 'activethingsalphalabel')),
        ('Things transparency (other modes):', slider('inactivethingsalpha', 'inactivethingsalphalabel')),
        ('Hidden things transparency:', slider('hiddenthingsalpha', 'hiddenthingsalphalabel')),
        ('Textures and flats brightness:', slider('imagebrightness', 'imagebrightnesslabel')),
        ('Dynamic lights count:', slider('tbDynLightCount', 'labelDynLightCount')),
        ('Anisotropic filtering:', slider('anisotropicfiltering', 'labelanisotropicfiltering')),
        ('Edge anti-aliasing:', slider('antialiasing', 'labelantialiasing')),
        (None, stack(chk('qualitydisplay'), chk('classicbilinear'), chk('visualbilinear'), chk('animatevisualselection'), chk('cbShowFPS'),
                     chk('cbStretchView'), chk('cbOldHighlightMode'), chk('cbMarkExtraFloors'), chk('cbFlatShadeVertices'),
                     chk('cbParallelizedLinedefPlotting'), chk('cbParallelizedVertexPlotting'))),
    ], 'lbl3')),
    widths=['290', '*'])

# ---- Script editor
script_colors = [('colorscriptbackground', 'Background'), ]
scriptedit = cols(
    card('Colors', rows([('Preset:', combo('scriptcolorpresets', 170))], 'lbl4') + stack(*[color(n) for n in [
        'colorproperties', 'colorscriptbackground', 'colorlinenumbers', 'colorplaintext', 'colorcomments', 'colorkeywords', 'colorstrings',
        'colorliterals', 'colorconstants', 'colorincludes', 'colorselectionfore', 'colorselectionback', 'colorindicator', 'colorbrace',
        'colorbracebad', 'colorwhitespace', 'colorfoldfore', 'colorfoldback']], spacing=4)),
    stack(
        card('Font', rows([('Font:', combo('scriptfontname', 220)), ('Size:', '<StackPanel Orientation="Horizontal" Spacing="12"><ComboBox x:Name="scriptfontsize" Width="90"/><CheckBox x:Name="scriptfontbold"/></StackPanel>')], 'lbl5')),
        card('Formatting', stack(chk('scriptusetabs'), '<StackPanel Orientation="Horizontal" Spacing="8"><TextBlock Text="Tab width:" Classes="uldb-field"/><ContentControl x:Name="scripttabwidth" Width="92"/></StackPanel>',
                                 chk('scriptautoindent'), chk('scriptautoclosebrackets'), chk('scriptallmanstyle'), chk('scriptautoshowautocompletion'))),
        card('Appearance', stack(chk('scriptshowlinenumbers'), chk('scriptshowfolding'))),
        card('Preview', '<Border Classes="uldb-grid" BorderThickness="1" CornerRadius="3" ClipToBounds="True" Height="170"><ContentControl x:Name="scriptedit"/></Border>').replace('<Border ', '<Border x:Name="previewgroup" ', 1),
    ), widths=['290', '*'])

# ---- Pasting
pasting = stack('<TextBlock x:Name="label16" TextWrapping="Wrap" Text="These are the default options for pasting geometry. You can also choose these options when you use the Paste Special function. These options also apply when inserting prefabs."/>', '<ContentControl x:Name="pasteoptions"/>', spacing=10)

# ---- Recovery
recovery = card('Autosave', chk('autosave'), '<Border x:Name="autosavedisabledwarning" Classes="uldb-group" Padding="8,4"><TextBlock Text="It is not recommended to disable autosaves!" Foreground="#E8A317"/></Border>', rows([
    ('Autosave interval:', slider('autosaveinterval', 'autosaveintervallabel', 200)),
    ('Number of autosaves:', slider('autosavecount', 'autosavecountlabel', 200)),
], 'lbl6'))

# ---- Toasts
toasts = stack(
    card('Toasts', chk('cbToastsEnabled'), rows([('Duration (seconds):', num('tbToastDuration'))], 'lbl7'),
         '<TextBlock Text="Toast position" Classes="uldb-field" Margin="0,6,0,0"/><UniformGrid Columns="2" Width="260" HorizontalAlignment="Left"><RadioButton x:Name="rbToastPosTL" GroupName="toastpos" Content="Top left"/><RadioButton x:Name="rbToastPosTR" GroupName="toastpos" Content="Top right"/><RadioButton x:Name="rbToastPosBL" GroupName="toastpos" Content="Bottom left"/><RadioButton x:Name="rbToastPosBR" GroupName="toastpos" Content="Bottom right"/></UniformGrid>'),
    card('Show toasts for', '<Border Classes="uldb-grid" BorderThickness="1" CornerRadius="3" ClipToBounds="True" Height="340"><ListBox x:Name="lvToastActions" SelectionMode="Single" Classes="uldb-checklist" ScrollViewer.HorizontalScrollBarVisibility="Auto"><ListBox.ItemTemplate><DataTemplate x:DataType="shell:ModelBinder+ListItem"><Grid ColumnDefinitions="280,Auto"><CheckBox IsChecked="{Binding Checked, Mode=OneWay}" Content="{Binding Text}"/><TextBlock Grid.Column="1" Text="{Binding SubText}" Opacity="0.7" VerticalAlignment="Center"/></Grid></DataTemplate></ListBox.ItemTemplate></ListBox></Border>'),
    spacing=10)

def tab(name, header, body, scrolling=True):
    content = scroll(body) if scrolling else body
    return '<TabItem x:Name="%s" Header="%s">%s</TabItem>' % (name, header, content)

xaml = '''<!-- ULDB: the window of upstream's PreferencesForm (model-ported: upstream's code runs unchanged).
     Controls named like upstream's fields are bound to them by ModelBinder.BindByName. Written by
     tools/axamlgen/preferences.py; labels upstream keeps in designer locals get their text there. -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:shell="using:ULDB.Shell"
        x:Class="CodeImp.DoomBuilder.Windows.PreferencesFormView"
        Title="Preferences" Width="1040" Height="760" MinWidth="880" MinHeight="620" ShowInTaskbar="False">
  <DockPanel Margin="14">
    <StackPanel DockPanel.Dock="Bottom" Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8" Margin="0,12,0,0">
      <Button x:Name="apply" IsDefault="True" MinWidth="100" HorizontalContentAlignment="Center" Classes="accent"/>
      <Button x:Name="cancel" IsCancel="True" MinWidth="100" HorizontalContentAlignment="Center"/>
    </StackPanel>
    <TabControl x:Name="tabs">
      %s
    </TabControl>
  </DockPanel>
</Window>
''' % '\n      '.join([
    tab('tabinterface', 'Interface', interface),
    tab('tabkeys', 'Controls', keys, False),
    tab('tabcolors', 'Appearance', appearance),
    tab('tabscripteditor', 'Script Editor', scriptedit),
    tab('tabpasting', 'Pasting', pasting),
    tab('tabrecovery', 'Recovery', recovery),
    tab('tabtoasts', 'Toasts', toasts),
])

# pretty print by splitting at tag boundaries
import re
xaml = re.sub(r'><(?!/)', '>\n<', xaml)
open(sys.argv[1], 'w').write(xaml)
