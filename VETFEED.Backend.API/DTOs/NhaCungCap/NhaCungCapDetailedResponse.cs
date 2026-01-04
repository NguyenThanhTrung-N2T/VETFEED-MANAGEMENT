using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;

namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    public class NhaCungCapDetailedResponse : NhaCungCapResponse
    {
        public List<NhaCungCapSanPhamResponse>? SanPhams { get; set; }
    }
}
