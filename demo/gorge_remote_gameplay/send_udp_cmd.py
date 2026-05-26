#!/usr/bin/env python3
"""Send play/stop/pause/seek/status/set-packages commands to the gorge_remote_player UDP listener.

Usage:
  python send_udp_cmd.py play                    # send {"cmd":"play"} to 127.0.0.1:9000
  python send_udp_cmd.py stop                    # send {"cmd":"stop"} to 127.0.0.1:9000
  python send_udp_cmd.py pause                   # send {"cmd":"pause"} to 127.0.0.1:9000
  python send_udp_cmd.py seek --seconds 12.5     # send {"cmd":"seek","seconds":12.5}
  python send_udp_cmd.py status                  # query status, wait for response
  python send_udp_cmd.py set-packages -r /abs/runtime.zip -c /abs/chart.zip
  python send_udp_cmd.py set-packages -r r1.zip -r r2.zip -c chart.zip

No dependencies beyond Python 3 standard library.
"""

import argparse
import json
import socket
from typing import Optional


def send_payload(host: str, port: int, payload: dict, timeout: float = 2.0) -> Optional[dict]:
    """Send a JSON payload via UDP. If timeout > 0, wait for a response and return it."""
    data = json.dumps(payload).encode("utf-8")
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.settimeout(timeout)
    try:
        sock.sendto(data, (host, port))
        print(f"Sent {json.dumps(payload)!r} to {host}:{port}")
        if timeout > 0:
            try:
                resp_data, addr = sock.recvfrom(4096)
                return json.loads(resp_data.decode("utf-8"))
            except socket.timeout:
                print(f"No response within {timeout}s")
                return None
        return None
    finally:
        sock.close()


def cmd_play(args: argparse.Namespace) -> None:
    send_payload(args.host, args.port, {"cmd": "play"}, timeout=0)


def cmd_stop(args: argparse.Namespace) -> None:
    send_payload(args.host, args.port, {"cmd": "stop"}, timeout=0)


def cmd_pause(args: argparse.Namespace) -> None:
    send_payload(args.host, args.port, {"cmd": "pause"}, timeout=0)


def cmd_seek(args: argparse.Namespace) -> None:
    send_payload(args.host, args.port, {"cmd": "seek", "seconds": args.seconds}, timeout=0)


def cmd_status(args: argparse.Namespace) -> None:
    response = send_payload(args.host, args.port, {"cmd": "status"}, timeout=args.timeout)
    if response is None:
        print("No status response received.")
        return
    if response.get("ok"):
        print(f"ChartTime:   {response['currentSeconds']:.3f}s")
        print(f"Duration:    {response['durationSeconds']:.3f}s")
        print(f"Begin:       {response['beginSeconds']:.3f}s")
        print(f"End:         {response['endSeconds']:.3f}s")
    else:
        print(f"Error: {response.get('error', 'unknown')}")


def cmd_set_packages(args: argparse.Namespace) -> None:
    payload = {"cmd": "set_packages"}
    if args.runtime:
        payload["runtimePackagePaths"] = args.runtime
    if args.chart:
        payload["chartPackagePaths"] = args.chart

    response = send_payload(args.host, args.port, payload, timeout=args.timeout)
    if response is None:
        print("No response received.")
        return
    if response.get("ok"):
        print(f"Runtime packages: {response.get('runtimePackagePaths', [])}")
        print(f"Chart packages:   {response.get('chartPackagePaths', [])}")
        print(f"Duration:         {response.get('durationSeconds', 0):.3f}s")
        print(f"Begin:            {response.get('beginSeconds', 0):.3f}s")
        print(f"End:              {response.get('endSeconds', 0):.3f}s")
    else:
        print(f"Error: {response.get('error', 'unknown')}")


def main() -> None:
    parser = argparse.ArgumentParser(description="Send UDP chart control commands")
    parser.add_argument("--host", default="127.0.0.1", help="Target host (default: 127.0.0.1)")
    parser.add_argument("--port", type=int, default=9000, help="Target port (default: 9000)")

    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("play", help='Send {"cmd":"play"}')
    sub.add_parser("stop", help='Send {"cmd":"stop"}')
    sub.add_parser("pause", help='Send {"cmd":"pause"}')

    seek_parser = sub.add_parser("seek", help='Send {"cmd":"seek","seconds":N}')
    seek_parser.add_argument(
        "--seconds", "-s", type=float, required=True,
        help="Target chart time in seconds (e.g. 12.5)"
    )

    status_parser = sub.add_parser("status", help='Send {"cmd":"status"} and print response')
    status_parser.add_argument(
        "--timeout", "-t", type=float, default=2.0,
        help="Response timeout in seconds (default: 2.0)"
    )

    setpkg_parser = sub.add_parser("set-packages", help='Send {"cmd":"set_packages"} with runtime/chart paths')
    setpkg_parser.add_argument(
        "--runtime", "-r", type=str, action="append", required=True,
        help="Runtime package path (repeat for multiple)"
    )
    setpkg_parser.add_argument(
        "--chart", "-c", type=str, action="append", required=True,
        help="Chart package path (repeat for multiple)"
    )
    setpkg_parser.add_argument(
        "--timeout", "-t", type=float, default=5.0,
        help="Response timeout in seconds (default: 5.0, for prepare time)"
    )

    args = parser.parse_args()

    handlers = {
        "play": cmd_play,
        "stop": cmd_stop,
        "pause": cmd_pause,
        "seek": cmd_seek,
        "status": cmd_status,
        "set-packages": cmd_set_packages,
    }
    handlers[args.command](args)


if __name__ == "__main__":
    main()
