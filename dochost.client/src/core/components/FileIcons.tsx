import {ReactElement} from "react";

type FileIconsProps = {
    type: "xls" | "xlsx" | "txt" | "jpg" | "png" | "pdf" | "doc" | "docx"
}

function FileIcons({ type }: FileIconsProps) {
    let icon: ReactElement;

    switch (type) {
        case "doc":
        case "docx":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                     stroke-linecap="round" stroke-linejoin="round"
                     className="text-blue-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M14 3v4a1 1 0 0 0 1 1h4"/>
                    <path d="M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4"/>
                    <path d="M5 15v6h1a2 2 0 0 0 2 -2v-2a2 2 0 0 0 -2 -2h-1z"/>
                    <path d="M20 16.5a1.5 1.5 0 0 0 -3 0v3a1.5 1.5 0 0 0 3 0"/>
                    <path
                        d="M12.5 15a1.5 1.5 0 0 1 1.5 1.5v3a1.5 1.5 0 0 1 -3 0v-3a1.5 1.5 0 0 1 1.5 -1.5z"/>
                </svg>
            );
            break;
        case "xls":
        case "xlsx":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-lime-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M14 3v4a1 1 0 0 0 1 1h4"/>
                    <path d="M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4"/>
                    <path d="M4 15l4 6"/>
                    <path d="M4 21l4 -6"/>
                    <path
                        d="M17 20.25c0 .414 .336 .75 .75 .75h1.25a1 1 0 0 0 1 -1v-1a1 1 0 0 0 -1 -1h-1a1 1 0 0 1 -1 -1v-1a1 1 0 0 1 1 -1h1.25a.75 .75 0 0 1 .75 .75"/>
                    <path d="M11 15v6h3"/>
                </svg>
            );
            break;
        case "jpg":
        case "png":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-sky-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M15 8h.01"/>
                    <path
                        d="M3 6a3 3 0 0 1 3 -3h12a3 3 0 0 1 3 3v12a3 3 0 0 1 -3 3h-12a3 3 0 0 1 -3 -3v-12z"/>
                    <path d="M3 16l5 -5c.928 -.893 2.072 -.893 3 0l5 5"/>
                    <path d="M14 14l1 -1c.928 -.893 2.072 -.893 3 0l3 3"/>
                </svg>
            );
            break;
        case "pdf":
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-red-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M14 3v4a1 1 0 0 0 1 1h4"/>
                    <path d="M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4"/>
                    <path d="M5 18h1.5a1.5 1.5 0 0 0 0 -3h-1.5v6"/>
                    <path d="M17 18h2"/>
                    <path d="M20 15h-3v6"/>
                    <path d="M11 15v6h1a2 2 0 0 0 2 -2v-2a2 2 0 0 0 -2 -2h-1z"/>
                </svg>
            );
            break;
        case "txt":
        default:
            icon = (
                <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24}
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2}
                     strokeLinecap="round" strokeLinejoin="round"
                     className="text-slate-600">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                    <path d="M14 3v4a1 1 0 0 0 1 1h4"/>
                    <path
                        d="M17 21h-10a2 2 0 0 1 -2 -2v-14a2 2 0 0 1 2 -2h7l5 5v11a2 2 0 0 1 -2 2z"/>
                    <path d="M9 9l1 0"/>
                    <path d="M9 13l6 0"/>
                    <path d="M9 17l6 0"/>
                </svg>
            );
            break;
    }

    return icon;
}

export default FileIcons