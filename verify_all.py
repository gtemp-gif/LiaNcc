from playwright.sync_api import sync_playwright
import time

def verify():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()

        endpoints = [
            ('/', 'verification/home_screenshot.png'),
            ('/Home/Fleet', 'verification/fleet_screenshot.png'),
            ('/Home/Tours', 'verification/tours_screenshot.png'),
            ('/Home/TourDetail', 'verification/tourdetail_screenshot.png'),
        ]

        for url_path, screenshot_path in endpoints:
            url = f"http://localhost:5286{url_path}"
            print(f"Loading {url}")
            page.goto(url)
            time.sleep(1) # wait for animations/images
            page.screenshot(path=screenshot_path, full_page=True)
            print(f"Saved {screenshot_path}")

        browser.close()

if __name__ == "__main__":
    verify()
