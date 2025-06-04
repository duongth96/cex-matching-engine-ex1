# src/main.py

from core.matching_engine import MatchingEngine
from core.order_book import OrderBook
from core.risk_checker import RiskChecker
from api.trading_api import TradingAPI
from models.order import Order, OrderType, OrderSide
import queue
import threading
import time

# Khởi tạo các thành phần cốt lõi
order_book = OrderBook()
risk_checker = RiskChecker()
matching_engine = MatchingEngine(order_book)

# Hàng đợi để nhận lệnh từ API
order_queue = queue.Queue()

def order_processing_worker():
    """
    Luồng xử lý lệnh liên tục từ hàng đợi
    """
    print("Order Processing Worker started...")
    while True:
        try:
            # Lấy lệnh từ hàng đợi (chặn nếu không có lệnh nào)
            order = order_queue.get()
            if order is None: # Sentinel for shutting down
                break

            print(f"Processing order: {order.order_id} ({order.side} {order.quantity} @ {order.price})")

            # 1. Kiểm tra rủi ro (giả định có hàm kiểm tra tài khoản người dùng)
            # is_valid, error_msg = risk_checker.check_order(order, user_account_info)
            # if not is_valid:
            #     print(f"Risk check failed for order {order.order_id}: {error_msg}")
            #     order_queue.task_done()
            #     continue
            print(f"Risk check passed for order {order.order_id}") # Giả định pass

            # 2. Chuyển lệnh đến Matching Engine
            trades = matching_engine.process_order(order)

            # 3. Xử lý kết quả (thông báo trades, cập nhật trạng thái)
            if trades:
                for trade in trades:
                    print(f"TRADE EXECUTED: {trade.instrument_id} {trade.quantity} @ {trade.price} "
                          f"({trade.buy_order_id} vs {trade.sell_order_id})")
                    # TODO: Cập nhật tài khoản người dùng, lưu trade vào DB, phát tán market data

            # Đánh dấu lệnh đã được xử lý xong
            order_queue.task_done()

        except Exception as e:
            print(f"Error in order processing worker: {e}")
            order_queue.task_done() # Vẫn đánh dấu để tránh tắc nghẽn

def start_api_server():
    """
    Hàm giả định khởi chạy API server để nhận lệnh và đưa vào queue
    Trong thực tế, TradingAPI sẽ là một Flask app, FastAPI, v.v.
    """
    api = TradingAPI(order_queue) # API nhận lệnh và đẩy vào queue
    print("API Server started (simulated)...")
    # Giả lập việc nhận lệnh từ API
    # Trong thực tế, đây là nơi Flask/FastAPI/WebSocket server chạy
    time.sleep(1) # Giả lập thời gian khởi động
    print("API ready to receive orders...")

    # Giả lập một vài lệnh đến từ API
    print("\n--- Simulating Incoming Orders ---")
    api.receive_order(Order(1, "BTC/USD", OrderSide.BUY, 29000.0, 0.5, OrderType.LIMIT))
    api.receive_order(Order(2, "BTC/USD", OrderSide.SELL, 29100.0, 1.0, OrderType.LIMIT))
    api.receive_order(Order(3, "BTC/USD", OrderSide.SELL, 28950.0, 0.3, OrderType.LIMIT)) # This one should match
    api.receive_order(Order(4, "BTC/USD", OrderSide.BUY, 29050.0, 0.2, OrderType.MARKET)) # This one should match
    api.receive_order(Order(5, "BTC/USD", OrderSide.SELL, 29050.0, 0.5, OrderType.LIMIT))
    api.receive_order(Order(6, "BTC/USD", OrderSide.BUY, 28900.0, 0.1, OrderType.LIMIT))
    api.receive_order(Order(7, "BTC/USD", OrderSide.SELL, 29000.0, 0.2, OrderType.LIMIT)) # Partial match with order 1
    print("--- Orders sent to queue ---")


if __name__ == "__main__":
    # Khởi tạo và chạy luồng xử lý lệnh
    processor_thread = threading.Thread(target=order_processing_worker)
    processor_thread.start()

    # Khởi chạy API server (có thể là một luồng khác nếu API là blocking)
    start_api_server()

    # Đợi cho tất cả các lệnh trong hàng đợi được xử lý xong
    order_queue.join() # Chờ cho tất cả task_done() được gọi

    # Gửi tín hiệu dừng cho luồng xử lý
    order_queue.put(None)
    processor_thread.join()

    print("\nSimulation Finished.")
    print("\nFinal Order Book Status:")
    print("BIDS:", order_book.bids_by_price)
    print("ASKS:", order_book.asks_by_price)