![enter image description here](https://i.imgur.com/mlUgail.jpg)

A lightweight, secure and open-source screenshare utility, written in C#, using no external library.
# Usage
## As a client
1 - Download Alabnet
2 - Type `alabnet connect [ip] [port] [pass] [nick]`
 - By default : `ip` is `localhost`, `port` is `80`, `pass` is (empty string), `nick` is `unknown`
 - `ip` will be resolved automatically

A window will open. If you double-tap it, it'll turn Topmost (i.e. always in the first plan in your screen)

## As a sender
 - Open `cmd` and type `ipconfig`
	 - Save your **local ip address** (ex: `192.168.0.31`)
 - Download UPNPC
	 - `A` ->the local ip address you saved earlier
	 - `B` -> the port **you'll specify** to alabnet, by default `80`
	 - `C` -> the port **you'll give** to clients, by default `80`
	 - `upnpc-shared -a A B C tcp`
		 - `upnpc-shared -a 192.168.0.31 80 80 tcp`
- Download Alabnet
	- `alabnet listen [port] [pass]`
