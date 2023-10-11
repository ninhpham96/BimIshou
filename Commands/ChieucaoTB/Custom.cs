using CommunityToolkit.Mvvm.ComponentModel;

namespace BimIshou.Commands.ChieucaoTB;

public partial class Custom : ObservableObject
{
    public int Id { get; set; }
    public string Name1 { get; set; }
    public string Name2 { get; set; }
    public double 距離 { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(平均高さ))]
    [NotifyPropertyChangedFor(nameof(面積))]
    private double _高さA;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(平均高さ))]
    [NotifyPropertyChangedFor(nameof(面積))]
    private double _高さB;

    public double 平均高さ => (高さB + 高さA) / 2;
    public double 面積 => ((高さB + 高さA) / 2) * 距離;
}
