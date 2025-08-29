// Mock data for the fashion store
const categories = [
    {
        id: 'women',
        name: 'Thời Trang Nữ',
        image: 'https://images.pexels.com/photos/1040945/pexels-photo-1040945.jpeg?auto=compress&cs=tinysrgb&w=800'
    },
    {
        id: 'men',
        name: 'Thời Trang Nam',
        image: 'https://images.pexels.com/photos/1043474/pexels-photo-1043474.jpeg?auto=compress&cs=tinysrgb&w=800'
    },
    {
        id: 'accessories',
        name: 'Phụ Kiện',
        image: 'https://images.pexels.com/photos/1503009/pexels-photo-1503009.jpeg?auto=compress&cs=tinysrgb&w=800'
    },
    {
        id: 'shoes',
        name: 'Giày Dép',
        image: 'https://images.pexels.com/photos/1464625/pexels-photo-1464625.jpeg?auto=compress&cs=tinysrgb&w=800'
    }
];

const products = [
    {
        id: 1,
        name: 'Áo Sơ Mi Trắng Classic',
        price: 899000,
        originalPrice: 1199000,
        image: 'https://images.pexels.com/photos/1566412/pexels-photo-1566412.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'women',
        description: 'Áo sơ mi trắng cổ điển với thiết kế thanh lịch, phù hợp cho công sở và dạo phố. Chất liệu cotton cao cấp, thoáng mát và dễ chăm sóc.',
        sizes: ['S', 'M', 'L', 'XL'],
        colors: ['Trắng', 'Xanh Nhạt', 'Hồng Pastel'],
        rating: 4.8,
        reviews: 124,
        isOnSale: true
    },
    {
        id: 2,
        name: 'Váy Midi Hoa Nhí',
        price: 1290000,
        image: 'https://images.pexels.com/photos/1040945/pexels-photo-1040945.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'women',
        description: 'Váy midi họa tiết hoa nhí dễ thương, chất liệu voan mềm mại thoáng mát. Thiết kế nữ tính, phù hợp cho các buổi hẹn hò và dạo phố.',
        sizes: ['S', 'M', 'L'],
        colors: ['Hoa Xanh', 'Hoa Hồng', 'Hoa Vàng'],
        rating: 4.6,
        reviews: 89,
        isNew: true
    },
    {
        id: 3,
        name: 'Áo Thun Nam Basic',
        price: 599000,
        image: 'https://images.pexels.com/photos/1043474/pexels-photo-1043474.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'men',
        description: 'Áo thun nam basic chất cotton 100%, form áo vừa vặn thoải mái. Thiết kế đơn giản, dễ phối đồ cho mọi hoạt động hàng ngày.',
        sizes: ['M', 'L', 'XL', 'XXL'],
        colors: ['Đen', 'Trắng', 'Xám', 'Navy'],
        rating: 4.7,
        reviews: 203
    },
    {
        id: 4,
        name: 'Quần Jean Skinny',
        price: 1590000,
        originalPrice: 1890000,
        image: 'https://images.pexels.com/photos/1598507/pexels-photo-1598507.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'men',
        description: 'Quần jean skinny cao cấp, chất denim co giãn thoải mái. Thiết kế ôm vừa phải, tôn dáng và phù hợp với nhiều phong cách.',
        sizes: ['29', '30', '31', '32', '33', '34'],
        colors: ['Xanh Đậm', 'Xanh Nhạt', 'Đen'],
        rating: 4.5,
        reviews: 156,
        isOnSale: true
    },
    {
        id: 5,
        name: 'Túi Xách Tay Cao Cấp',
        price: 2390000,
        image: 'https://images.pexels.com/photos/1503009/pexels-photo-1503009.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'accessories',
        description: 'Túi xách tay da cao cấp, thiết kế sang trọng với nhiều ngăn tiện lợi. Phù hợp cho công sở và các dịp quan trọng.',
        sizes: ['One Size'],
        colors: ['Đen', 'Nâu', 'Beige'],
        rating: 4.9,
        reviews: 67,
        isNew: true
    },
    {
        id: 6,
        name: 'Giày Sneaker Trắng',
        price: 1890000,
        image: 'https://images.pexels.com/photos/1464625/pexels-photo-1464625.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'shoes',
        description: 'Giày sneaker trắng minimalist, đế cao su chống trượt thoải mái. Thiết kế đơn giản, dễ phối đồ cho mọi outfit.',
        sizes: ['37', '38', '39', '40', '41', '42', '43'],
        colors: ['Trắng', 'Kem'],
        rating: 4.4,
        reviews: 98
    },
    {
        id: 7,
        name: 'Blazer Nữ Thanh Lịch',
        price: 1990000,
        image: 'https://images.pexels.com/photos/1036622/pexels-photo-1036622.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'women',
        description: 'Áo blazer nữ cắt may tinh tế, phù hợp cho môi trường công sở. Chất liệu cao cấp, form dáng thanh lịch và chuyên nghiệp.',
        sizes: ['S', 'M', 'L', 'XL'],
        colors: ['Đen', 'Navy', 'Be'],
        rating: 4.7,
        reviews: 145
    },
    {
        id: 8,
        name: 'Áo Hoodie Nam',
        price: 1290000,
        originalPrice: 1590000,
        image: 'https://images.pexels.com/photos/1300402/pexels-photo-1300402.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'men',
        description: 'Áo hoodie nam phong cách streetwear, chất nỉ ấm áp. Thiết kế trẻ trung, phù hợp cho thời tiết se lạnh.',
        sizes: ['M', 'L', 'XL', 'XXL'],
        colors: ['Đen', 'Xám', 'Navy', 'Trắng'],
        rating: 4.6,
        reviews: 187,
        isOnSale: true
    },
    {
        id: 9,
        name: 'Chân Váy Xếp Ly',
        price: 890000,
        image: 'https://images.pexels.com/photos/1536619/pexels-photo-1536619.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'women',
        description: 'Chân váy xếp ly nữ tính, chất liệu vải mềm mại. Thiết kế trẻ trung, phù hợp cho học sinh, sinh viên và các cô gái yêu thích phong cách ngọt ngào.',
        sizes: ['S', 'M', 'L'],
        colors: ['Đen', 'Navy', 'Kẻ Caro'],
        rating: 4.5,
        reviews: 92
    },
    {
        id: 10,
        name: 'Đồng Hồ Nam Thể Thao',
        price: 3490000,
        image: 'https://images.pexels.com/photos/190819/pexels-photo-190819.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'accessories',
        description: 'Đồng hồ nam thể thao chống nước, thiết kế mạnh mẽ và năng động. Tính năng đa dạng, phù hợp cho các hoạt động thể thao và hàng ngày.',
        sizes: ['One Size'],
        colors: ['Đen', 'Xanh', 'Bạc'],
        rating: 4.8,
        reviews: 156,
        isNew: true
    },
    {
        id: 11,
        name: 'Giày Cao Gót Nữ',
        price: 1690000,
        originalPrice: 1990000,
        image: 'https://images.pexels.com/photos/336372/pexels-photo-336372.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'shoes',
        description: 'Giày cao gót nữ thanh lịch, gót cao 7cm vừa phải. Thiết kế sang trọng, phù hợp cho các buổi tiệc và sự kiện quan trọng.',
        sizes: ['35', '36', '37', '38', '39'],
        colors: ['Đen', 'Nude', 'Đỏ'],
        rating: 4.3,
        reviews: 78,
        isOnSale: true
    },
    {
        id: 12,
        name: 'Áo Khoác Bomber Nam',
        price: 1790000,
        image: 'https://images.pexels.com/photos/1183266/pexels-photo-1183266.jpeg?auto=compress&cs=tinysrgb&w=800',
        category: 'men',
        description: 'Áo khoác bomber nam phong cách streetwear, chất liệu dù chống gió nhẹ. Thiết kế trẻ trung, năng động phù hợp cho các chàng trai yêu thích phong cách urban.',
        sizes: ['M', 'L', 'XL', 'XXL'],
        colors: ['Đen', 'Xanh Rêu', 'Nâu'],
        rating: 4.6,
        reviews: 134
    }
];

// Utility functions
function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
}

function generateStars(rating) {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 !== 0;
    let starsHtml = '';

    for (let i = 0; i < fullStars; i++) {
        starsHtml += '★';
    }

    if (hasHalfStar) {
        starsHtml += '☆';
    }

    const emptyStars = 5 - Math.ceil(rating);
    for (let i = 0; i < emptyStars; i++) {
        starsHtml += '☆';
    }

    return starsHtml;
}