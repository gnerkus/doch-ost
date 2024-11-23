import {ReactElement} from "react";

type UIIconsProps = {
    type: "share" | "download" | "user" | "upload" | "copy"
}

function UIIcons({ type }: UIIconsProps) {
    let icon: ReactElement | null;

    switch (type) {
        case "copy":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-slate-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path
                        d="M7 7m0 2.667a2.667 2.667 0 0 1 2.667 -2.667h8.666a2.667 2.667 0 0 1 2.667 2.667v8.666a2.667 2.667 0 0 1 -2.667 2.667h-8.666a2.667 2.667 0 0 1 -2.667 -2.667z"/>
                    <path
                        d="M4.012 16.737a2.005 2.005 0 0 1 -1.012 -1.737v-10c0 -1.1 .9 -2 2 -2h10c.75 0 1.158 .385 1.5 1"/>
                </svg>
            );
            break;
        case "download":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-slate-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2 -2v-2"/>
                    <path d="M7 11l5 5l5 -5"/>
                    <path d="M12 4l0 12"/>
                </svg>
            );
            break;
        case "share":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-slate-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path
                        d="M13 4v4c-6.575 1.028 -9.02 6.788 -10 12c-.037 .206 5.384 -5.962 10 -6v4l8 -7l-8 -7z"/>
                </svg>
            );
            break;
        case "upload":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-white">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2 -2v-2"/>
                    <path d="M7 9l5 -5l5 5"/>
                    <path d="M12 4l0 12"/>
                </svg>
            );
            break;
        case "user":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-slate-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M8 7a4 4 0 1 0 8 0a4 4 0 0 0 -8 0"/>
                    <path d="M6 21v-2a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v2"/>
                </svg>
            );
            break;
        default:
            icon = null;
    }

    return icon;
}

export default UIIcons