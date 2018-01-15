import PropTypes from "prop-types";
import {TaskShape} from "./TaskShape";

export const ChallengeShape = PropTypes.shape({
    id: PropTypes.number.isRequired,
    names: PropTypes.shape({
        male: PropTypes.string.isRequired,
        female: PropTypes.string.isRequired,
    }).isRequired,
    category: PropTypes.number.isRequired,

    requirements: PropTypes.shape({
        basic: PropTypes.shape({
            cubs: PropTypes.number,
            scouts: PropTypes.number,
            guides: PropTypes.number,
        }).isRequired,
        extra: PropTypes.shape({
            cubs: PropTypes.number,
            scouts: PropTypes.number,
            guides: PropTypes.number,
        }).isRequired,
    }).isRequired,
    imageUri: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
    tasks: PropTypes.shape({
        basic: PropTypes.arrayOf(TaskShape).isRequired,
        extra: PropTypes.arrayOf(TaskShape).isRequired,
    })
});