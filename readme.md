# project structure
Project_Trading_System

$ tree
.
├── src/
│   ├── main.cs                     # Điểm khởi chạy chính của hệ thống
│   ├── models/                     # Chứa định nghĩa các đối tượng dữ liệu
│   │   ├── order.cs                # Định nghĩa lớp Order (lệnh)
│   │   └── trade.cs                # Định nghĩa lớp Trade (giao dịch đã khớp)
│   ├── core/                       # Các thành phần cốt lõi của hệ thống
│   │   ├── order_book.cs           # Quản lý Sổ lệnh (Bids và Asks)
│   │   ├── matching_engine.cs      # Khớp lệnh và xử lý giao dịch
│   │   ├── risk_checker.cs         # Kiểm tra rủi ro (tùy chọn nhưng quan trọng)
│   │   └── market_data_publisher.cs # Phát tán dữ liệu thị trường (tùy chọn)
│   ├── api/                        # Giao diện tiếp nhận lệnh từ người dùng
│   │   └── trading_api.cs          # API để nhận và gửi lệnh (ví dụ: REST, WebSocket)
│   ├── utils/                      # Các hàm tiện ích chung
│   │   ├── logger.cs               # Hệ thống ghi log
│   │   └── constants.cs            # Các hằng số của hệ thống


# Giải thích chi tiết các thành phần chính:
## 1. src/models/

    order.cs:
        Định nghĩa lớp Order (Lệnh giao dịch).
        Các thuộc tính cơ bản của một lệnh:
            order_id: ID duy nhất của lệnh.
            instrument_id: Mã tài sản (ví dụ: AAPL, BTC/USD).
            side: Loại lệnh (BUY - Mua / SELL - Bán).
            price: Giá đặt lệnh (đối với lệnh giới hạn) hoặc None (đối với lệnh thị trường).
            quantity: Khối lượng đặt lệnh.
            order_type: Loại lệnh (LIMIT - Giới hạn / MARKET - Thị trường / STOP - Dừng, v.v.).
            timestamp: Thời gian lệnh được tạo.
            status: Trạng thái hiện tại của lệnh (PENDING, PARTIALLY_FILLED, FILLED, CANCELED).
            filled_quantity: Khối lượng đã khớp.
            remaining_quantity: Khối lượng còn lại.
        Các phương thức: Khởi tạo, cập nhật trạng thái, v.v.

    trade.cs:
        Định nghĩa lớp Trade (Giao dịch đã khớp).
        Các thuộc tính:
            trade_id: ID duy nhất của giao dịch.
            instrument_id: Mã tài sản.
            price: Giá thực hiện giao dịch.
            quantity: Khối lượng đã khớp.
            buy_order_id: ID của lệnh mua đã tham gia.
            sell_order_id: ID của lệnh bán đã tham gia.
            timestamp: Thời gian giao dịch được khớp.

## 2. src/core/

    order_book.cs:
        Lớp OrderBook để quản lý sổ lệnh.
        Thành phần chính:
            bids: Một cấu trúc dữ liệu (ví dụ: SortedDict hoặc dict chứa list/deque) để lưu trữ các lệnh mua, được sắp xếp theo giá giảm dần (giá cao nhất ưu tiên).
            asks: Một cấu trúc dữ liệu tương tự để lưu trữ các lệnh bán, được sắp xếp theo giá tăng dần (giá thấp nhất ưu tiên).
        Các phương thức:
            add_order(order): Thêm một lệnh mới vào bids hoặc asks.
            remove_order(order_id): Xóa một lệnh.
            get_best_bid(): Lấy lệnh mua có giá cao nhất.
            get_best_ask(): Lấy lệnh bán có giá thấp nhất.
            get_depth(): Trả về độ sâu thị trường (tổng khối lượng ở mỗi mức giá).
            update_order(order_id, filled_quantity): Cập nhật trạng thái của lệnh sau khi khớp một phần.

    matching_engine.cs:
        Lớp MatchingEngine.
        Phụ thuộc: Sử dụng một thể hiện của OrderBook.
        Phương thức chính:
            process_order(order): Phương thức nhận một lệnh mới và cố gắng khớp nó.
                Logic:
                    Kiểm tra loại lệnh (Market/Limit).
                    Nếu là lệnh BUY: Tìm kiếm trong asks để tìm lệnh bán có giá ≤ giá lệnh mua (đối với Limit) hoặc giá tốt nhất (đối với Market).
                    Nếu là lệnh SELL: Tìm kiếm trong bids để tìm lệnh mua có giá ≥ giá lệnh bán (đối với Limit) hoặc giá tốt nhất (đối với Market).
                    Thực hiện khớp lệnh theo quy tắc ưu tiên (Price-Time Priority).
                    Tạo các đối tượng Trade cho mỗi giao dịch đã khớp.
                    Cập nhật trạng thái của các lệnh đã khớp (hoàn toàn hoặc một phần) trong OrderBook.
                    Nếu lệnh chưa khớp hết, thêm phần còn lại vào OrderBook.
                    Trả về danh sách các Trade đã được tạo.

    risk_checker.cs (Tùy chọn nhưng rất quan trọng cho hệ thống thực tế):
        Lớp RiskChecker.
        Vai trò: Kiểm tra các điều kiện rủi ro trước khi lệnh được gửi đến Matching Engine hoặc sau khi lệnh được tạo.
        Các kiểm tra có thể có:
            Đủ tiền/ký quỹ.
            Giới hạn khối lượng lệnh tối đa/tối thiểu.
            Giới hạn vị thế (Exposure limit).
            Kiểm tra giá bất thường.
        Phương thức: check_order(order, account_info): Trả về True nếu lệnh hợp lệ, False nếu không.

    market_data_publisher.cs (Tùy chọn):
        Lớp để phát tán dữ liệu thị trường theo thời gian thực (ví dụ: giá khớp, độ sâu sổ lệnh) đến các client hoặc hệ thống khác.
        Sử dụng WebSocket hoặc Kafka để truyền dữ liệu.

## 3. src/api/

    trading_api.cs:
        Xây dựng giao diện để client (người dùng) có thể tương tác với hệ thống.
        Có thể là một API RESTful để nhận lệnh (POST /order) và truy vấn thông tin (GET /order_book, GET /trades).
        Hoặc một giao thức WebSocket để nhận các cập nhật dữ liệu thị trường theo thời gian thực.
        API này sẽ nhận lệnh từ client, có thể gọi RiskChecker trước, sau đó chuyển lệnh hợp lệ đến MatchingEngine.

## 4. src/main.cs

    Điểm khởi chạy chính của ứng dụng.
    Khởi tạo các đối tượng cốt lõi: OrderBook, MatchingEngine, RiskChecker.
    Khởi chạy TradingAPI để bắt đầu lắng nghe các lệnh từ client.
    Thiết lập vòng lặp chính để xử lý các sự kiện (lệnh mới, hủy lệnh, v.v.).

## 5. src/tests/

    test_order_book.cs: Các unit test cho OrderBook (kiểm tra thêm/xóa lệnh, sắp xếp, lấy best bid/ask).
    test_matching_engine.cs: Các unit test cho MatchingEngine (kiểm tra các kịch bản khớp lệnh khác nhau: khớp hoàn toàn, khớp một phần, không khớp, lệnh thị trường, lệnh giới hạn).
    test_e2e_trading.cs: Các bài kiểm thử end-to-end mô phỏng luồng giao dịch từ gửi lệnh qua API đến khi lệnh được xử lý và giao dịch được tạo.

Quy trình Luồng Dữ liệu (Simplified Flow)

    Client gửi lệnh: Người dùng gửi lệnh mua/bán thông qua TradingAPI.
    Kiểm tra rủi ro: TradingAPI có thể chuyển lệnh qua RiskChecker để đảm bảo lệnh hợp lệ.
    Xử lý lệnh: Nếu lệnh hợp lệ, TradingAPI gửi lệnh đó đến MatchingEngine.
    Khớp lệnh: MatchingEngine nhận lệnh, tìm kiếm trong OrderBook (Bids và Asks) để tìm lệnh đối ứng phù hợp.
    Tạo giao dịch: Nếu lệnh được khớp, MatchingEngine tạo một đối tượng Trade và cập nhật OrderBook (giảm khối lượng lệnh đã khớp, xóa lệnh nếu khớp hoàn toàn).
    Thông báo: MarketDataPublisher (nếu có) sẽ phát tán thông tin về giao dịch mới hoặc cập nhật độ sâu sổ lệnh cho các client.
    Cập nhật trạng thái: Trạng thái của lệnh gốc và tài khoản của người dùng được cập nhật.


# Giải thuật
HÀM order_processing_worker():
    IN: Không có
    OUT: Không có

    KHỞI TẠO:
        engine = tham chiếu đến MatchingEngine instance
        risk_checker = tham chiếu đến RiskChecker instance (nếu có)
        order_queue = tham chiếu đến hàng đợi lệnh

    VÒNG LẶP VÔ HẠN (while True):
        CỐ GẮNG (try):
            # 1. Lấy lệnh từ hàng đợi
            order = order_queue.get()

            # Kiểm tra tín hiệu dừng (ví dụ: None)
            NẾU order IS None THÌ
                THOÁT VÒNG LẶP
            KẾT THÚC NẾU

            GHI LOG: "Đang xử lý lệnh [order.id]"

            # 2. Kiểm tra rủi ro (Bước tùy chọn nhưng rất quan trọng)
            # Giả định risk_checker.check_order() trả về True/False và message
            # NẾU risk_checker CÓ TỒN TẠI VÀ KHÔNG (risk_checker.check_order(order, order.user_id)) THÌ
            #     GHI LOG: "Lệnh [order.id] không hợp lệ: [lý do]"
            #     order_queue.task_done()
            #     TIẾP TỤC VÒNG LẶP (bỏ qua lệnh này)
            # KẾT THÚC NẾU
            # GHI LOG: "Kiểm tra rủi ro cho lệnh [order.id] đã qua"

            # 3. Chuyển lệnh cho Matching Engine để khớp
            trades = engine.process_order(order)

            # 4. Xử lý kết quả trả về từ Matching Engine
            NẾU trades CÓ DỮ LIỆU THÌ
                VỚI MỖI trade TRONG trades:
                    GHI LOG: "Đã khớp giao dịch: [trade.quantity] của [trade.instrument_id] tại giá [trade.price]"
                    # TODO:
                    #   - Cập nhật tài khoản của người dùng (tính toán lãi/lỗ, thay đổi số dư)
                    #   - Gửi thông báo giao dịch đã khớp tới các bên liên quan (người dùng, hệ thống Market Data Publisher)
                    #   - Lưu giao dịch vào cơ sở dữ liệu lịch sử
            KẾT THÚC NẾU

            # 5. Đánh dấu lệnh đã được xử lý xong trong hàng đợi
            order_queue.task_done()

        BẮT LỖI (except Exception as e):
            GHI LOG LỖI: "Lỗi trong quá trình xử lý lệnh: [e]"
            # Quan trọng: Đảm bảo task_done() được gọi ngay cả khi có lỗi để tránh tắc nghẽn hàng đợi
            order_queue.task_done()
    KẾT THÚC VÒNG LẶP
KẾT THÚC HÀM

HÀM MatchingEngine.process_order(order):
    IN: order (đối tượng Order mới)
    OUT: Danh sách các đối tượng Trade đã khớp

    KHỞI TẠO:
        order_book = tham chiếu đến OrderBook của Matching Engine
        trades = danh sách rỗng để lưu trữ các giao dịch đã khớp
        remaining_quantity = order.quantity # Khối lượng còn lại của lệnh mới cần khớp

    # Cập nhật trạng thái lệnh ban đầu (đang chờ xử lý)
    order.status = PENDING

    # PHẦN 1: XỬ LÝ LỆNH THỊ TRƯỜNG (MARKET ORDER)
    NẾU order.order_type LÀ MARKET THÌ
        NẾU order.side LÀ BUY THÌ
            # Tìm kiếm trong ASKS BOOK (lệnh bán) để khớp
            VÒNG LẶP VÔ HẠN (WHILE remaining_quantity > 0):
                best_ask = order_book.get_best_ask()
                NẾU best_ask IS None HOẶC best_ask.price > order.price (nếu lệnh thị trường có giới hạn giá, hiếm) THÌ
                    THOÁT VÒNG LẶP (không còn lệnh bán để khớp)
                KẾT THÚC NẾU

                # Tính toán khối lượng khớp
                match_quantity = MIN(remaining_quantity, best_ask.remaining_quantity)

                # Thực hiện khớp
                trade = TẠO_TRADE_MỚI(order.instrument_id, best_ask.price, match_quantity, order.id, best_ask.id)
                THÊM trade VÀO trades

                # Cập nhật khối lượng còn lại của lệnh mới
                remaining_quantity = remaining_quantity - match_quantity

                # Cập nhật trạng thái của lệnh đã khớp trong Order Book
                order_book.update_order(best_ask.id, match_quantity)

                # Nếu lệnh bán đã khớp hoàn toàn, xóa khỏi Order Book
                NẾU best_ask.remaining_quantity LÀ 0 THÌ
                    order_book.remove_order(best_ask.id)
                KẾT THÚC NẾU
        KẾT THÚC NẾU

        NẾU order.side LÀ SELL THÌ
            # Tìm kiếm trong BIDS BOOK (lệnh mua) để khớp
            VÒNG LẶP VÔ HẠN (WHILE remaining_quantity > 0):
                best_bid = order_book.get_best_bid()
                NẾU best_bid IS None HOẶC best_bid.price < order.price (nếu lệnh thị trường có giới hạn giá, hiếm) THÌ
                    THOÁT VÒNG LẶP (không còn lệnh mua để khớp)
                KẾT THÚC NẾU

                # Tính toán khối lượng khớp
                match_quantity = MIN(remaining_quantity, best_bid.remaining_quantity)

                # Thực hiện khớp
                trade = TẠO_TRADE_MỚI(order.instrument_id, best_bid.price, match_quantity, best_bid.id, order.id)
                THÊM trade VÀO trades

                # Cập nhật khối lượng còn lại của lệnh mới
                remaining_quantity = remaining_quantity - match_quantity

                # Cập nhật trạng thái của lệnh đã khớp trong Order Book
                order_book.update_order(best_bid.id, match_quantity)

                # Nếu lệnh mua đã khớp hoàn toàn, xóa khỏi Order Book
                NẾU best_bid.remaining_quantity LÀ 0 THÌ
                    order_book.remove_order(best_bid.id)
                KẾT THÚC NẾU
        KẾT THÚC NẾU
    KẾT THÚC NẾU

    # PHẦN 2: XỬ LÝ LỆNH GIỚI HẠN (LIMIT ORDER)
    NẾU order.order_type LÀ LIMIT THÌ
        NẾU order.side LÀ BUY THÌ
            # Tìm kiếm trong ASKS BOOK (lệnh bán) để khớp
            VÒNG LẶP VÔ HẠN (WHILE remaining_quantity > 0):
                best_ask = order_book.get_best_ask()
                NẾU best_ask IS None HOẶC best_ask.price > order.price THÌ
                    THOÁT VÒNG LẶP (không còn lệnh bán phù hợp)
                KẾT THÚC NẾU

                # Tính toán khối lượng khớp
                match_quantity = MIN(remaining_quantity, best_ask.remaining_quantity)

                # Thực hiện khớp
                trade = TẠO_TRADE_MỚI(order.instrument_id, best_ask.price, match_quantity, order.id, best_ask.id)
                THÊM trade VÀO trades

                # Cập nhật khối lượng còn lại của lệnh mới
                remaining_quantity = remaining_quantity - match_quantity

                # Cập nhật trạng thái của lệnh đã khớp trong Order Book
                order_book.update_order(best_ask.id, match_quantity)

                # Nếu lệnh bán đã khớp hoàn toàn, xóa khỏi Order Book
                NẾU best_ask.remaining_quantity LÀ 0 THÌ
                    order_book.remove_order(best_ask.id)
                KẾT THÚC NẾU
        KẾT THÚC NẾU

        NẾU order.side LÀ SELL THÌ
            # Tìm kiếm trong BIDS BOOK (lệnh mua) để khớp
            VÒNG LẶP VÔ HẠN (WHILE remaining_quantity > 0):
                best_bid = order_book.get_best_bid()
                NẾU best_bid IS None HOẶC best_bid.price < order.price THÌ
                    THOÁT VÒNG LẶP (không còn lệnh mua phù hợp)
                KẾT THÚC NẾU

                # Tính toán khối lượng khớp
                match_quantity = MIN(remaining_quantity, best_bid.remaining_quantity)

                # Thực hiện khớp
                trade = TẠO_TRADE_MỚI(order.instrument_id, best_bid.price, match_quantity, best_bid.id, order.id)
                THÊM trade VÀO trades

                # Cập nhật khối lượng còn lại của lệnh mới
                remaining_quantity = remaining_quantity - match_quantity

                # Cập nhật trạng thái của lệnh đã khớp trong Order Book
                order_book.update_order(best_bid.id, match_quantity)

                # Nếu lệnh mua đã khớp hoàn toàn, xóa khỏi Order Book
                NẾU best_bid.remaining_quantity LÀ 0 THÌ
                    order_book.remove_order(best_bid.id)
                KẾT THÚC NẾU
        KẾT THÚC NẾU
    KẾT THÚC NẾU

    # PHẦN 3: XỬ LÝ LỆNH SAU KHI THỬ KHỚP
    order.filled_quantity = order.quantity - remaining_quantity
    order.remaining_quantity = remaining_quantity

    NẾU remaining_quantity LÀ 0 THÌ
        order.status = FILLED # Lệnh đã khớp hoàn toàn
    NGƯỢC LẠI NẾU remaining_quantity < order.quantity THÌ
        order.status = PARTIALLY_FILLED # Lệnh khớp một phần
    NGƯỢC LẠI (remaining_quantity LÀ order.quantity) THÌ
        order.status = PENDING # Lệnh không khớp được gì

    # Nếu lệnh giới hạn chưa khớp hết, thêm phần còn lại vào Order Book
    NẾU order.order_type LÀ LIMIT VÀ remaining_quantity > 0 THÌ
        order_book.add_order(order)
    KẾT THÚC NẾU

    TRẢ VỀ trades
KẾT THÚC HÀM