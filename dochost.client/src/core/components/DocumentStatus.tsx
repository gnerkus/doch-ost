import UIIcons from "./UIIcons.tsx";
import LoadingIcon from "./LoadingIcon.tsx";

type DocumentStatusProps = {
    status: "completed" | "failed" | "processing" | "queued" | ""
}

function DocumentStatus({ status}: DocumentStatusProps) {

    let icon = null;

    switch (status) {
        case "failed":
            icon = <UIIcons type="error" />;
            break;
        case "completed":
            icon = null;
            break;
        case "queued":
        case "processing":
            icon = <LoadingIcon size={24}/>;
            break;
        default:
            icon = <UIIcons type="warning" />;
            break;
    }

    return icon;
}

export default DocumentStatus