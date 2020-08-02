![enter image description here](https://i.imgur.com/mlUgail.jpg)

A lightweight, secure and open-source screenshare utility, written in C#, using no external library.

# Notes
 - At the moment, the encryption doesn't work properly. It is disabled temporarily.
- At the moment, you'll only find sort of "pre-releases", with lots of bugs, but still quite good for simple screenshare.

# Usage
## As a client
- Download Alabnet
	- `alabnet connect [ip] [port] [pass] [nick]`
	 - By default : `ip` is `localhost`, `port` is `80`, `pass` is (empty string), `nick` is `unknown`
	- `ip` will be resolved automatically

If the sender accepts your connection, a window will open.
If you double-tap it, it'll turn Topmost (i.e. always in the first plan in your screen)

## As a sender
 - Open `cmd` and type `ipconfig`
	 - Save your **local ip address** (ex: `192.168.0.31`)
 - Download UPNPC
	 - `A` ->the local ip address you saved earlier
	 - `B` -> the port **you'll specify** to alabnet, by default `80`
	 - `C` -> the port **you'll give** to clients, by default `80`
	 - `upnpc-shared -a A B C tcp`
	- Whenever you are done with the screenshare, you might close the UpNp port
		- `upnpc-shared -d [port] tcp`
- Download Alabnet
	- `alabnet listen [port] [pass]`